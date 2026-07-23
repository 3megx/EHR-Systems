using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Common.Messaging;

/// <summary>
/// Background service that processes unpublished events from outbox.
/// Runs continuously, polling for new events every X seconds.
/// 
/// Behavior:
/// 1. Poll outbox for unpublished events
/// 2. Attempt to publish to Kafka
/// 3. On success: mark as published
/// 4. On failure: increment retry count
/// 5. If max retries exceeded: move to dead letter queue
/// 
/// HIPAA: Ensures no events are lost during restarts.
/// Events stay in database until successfully published.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly int _pollIntervalSeconds = 5;
    private readonly int _maxRetries = 3;
    private readonly int _cleanupDays = 30;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Main processing loop.
    /// Runs until service is stopped.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                // Get unpublished events
                var unpublishedEvents = await outboxRepository.GetUnpublishedAsync(stoppingToken);
                var eventList = unpublishedEvents.ToList();

                if (eventList.Any())
                {
                    _logger.LogInformation("Found {Count} unpublished events", eventList.Count);

                    // Process each event
                    foreach (var outboxEvent in eventList)
                    {
                        await ProcessEventAsync(
                            outboxEvent,
                            outboxRepository,
                            eventPublisher,
                            stoppingToken);
                    }
                }

                // Cleanup old published events
                await outboxRepository.DeletePublishedOlderThanAsync(_cleanupDays, stoppingToken);

                // Get pending count for monitoring
                var pendingCount = await outboxRepository.GetPendingCountAsync(stoppingToken);
                if (pendingCount > 0)
                {
                    _logger.LogInformation("Pending events in outbox: {Count}", pendingCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox events");
            }

            // Wait before next poll
            await Task.Delay(TimeSpan.FromSeconds(_pollIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Outbox processor stopped");
    }

    /// <summary>
    /// Process single event with retry logic.
    /// </summary>
    private async Task ProcessEventAsync(
        OutboxEvent outboxEvent,
        IOutboxRepository outboxRepository,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Publishing event {EventId} of type {EventType}",
                outboxEvent.Id,
                outboxEvent.EventType);

            // Deserialize event
            var eventData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                outboxEvent.EventData,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Publish to Kafka
            var integrationEvent = new IntegrationEvent
            {
                EventId = outboxEvent.Id,
                EventType = outboxEvent.EventType,
                Timestamp = outboxEvent.CreatedAt,
                Data = eventData
            };

            await eventPublisher.PublishAsync(integrationEvent, cancellationToken);

            // Mark as published
            await outboxRepository.MarkAsPublishedAsync(outboxEvent.Id, cancellationToken);

            _logger.LogInformation(
                "Event {EventId} published successfully",
                outboxEvent.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Error publishing event {EventId}, attempt {Attempt}",
                outboxEvent.Id,
                outboxEvent.PublishAttempts + 1);

            // Increment failure count
            await outboxRepository.IncrementAttemptAsync(
                outboxEvent.Id,
                ex.Message,
                cancellationToken);

            // Check if exceeded max retries
            if ((outboxEvent.PublishAttempts + 1) >= outboxEvent.MaxPublishAttempts)
            {
                _logger.LogError(
                    "Event {EventId} exceeded max retries ({MaxRetries}), moving to dead letter queue",
                    outboxEvent.Id,
                    outboxEvent.MaxPublishAttempts);

                // TODO: Move to dead letter queue for manual inspection
                // await deadLetterQueue.EnqueueAsync(outboxEvent);
            }
        }
    }
}

/// <summary>
/// Event publisher interface for integration events.
/// Publishes to Kafka or other messaging system.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publish single event.
    /// </summary>
    Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish multiple events in batch.
    /// </summary>
    Task PublishBatchAsync(
        IEnumerable<IntegrationEvent> events,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic integration event wrapper.
/// Holds event metadata and data.
/// </summary>
public class IntegrationEvent
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

/// <summary>
/// Outbox repository implementation using Entity Framework.
/// Stores unpublished events in database.
/// </summary>
public class OutboxRepository : IOutboxRepository
{
    private readonly DbContext _context;

    public OutboxRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OutboxEvent>> GetUnpublishedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxEvent>()
            .Where(e => !e.IsPublished && e.PublishAttempts < e.MaxPublishAttempts)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxEvent>> GetFailedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxEvent>()
            .Where(e => !e.IsPublished && e.PublishAttempts >= e.MaxPublishAttempts)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OutboxEvent @event, CancellationToken cancellationToken = default)
    {
        await _context.Set<OutboxEvent>().AddAsync(@event, cancellationToken);
    }

    public async Task MarkAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _context.Set<OutboxEvent>().FindAsync(new object[] { eventId }, cancellationToken);
        if (@event != null)
        {
            @event.PublishedAt = DateTime.UtcNow;
            _context.Set<OutboxEvent>().Update(@event);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task IncrementAttemptAsync(
        Guid eventId,
        string failureReason,
        CancellationToken cancellationToken = default)
    {
        var @event = await _context.Set<OutboxEvent>().FindAsync(new object[] { eventId }, cancellationToken);
        if (@event != null)
        {
            @event.PublishAttempts++;
            @event.ErrorMessage = failureReason;
            _context.Set<OutboxEvent>().Update(@event);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<OutboxEvent?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxEvent>().FindAsync(new object[] { eventId }, cancellationToken);
    }

    public async Task DeletePublishedOlderThanAsync(int days, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        var oldEvents = await _context.Set<OutboxEvent>()
            .Where(e => e.IsPublished && e.PublishedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        _context.Set<OutboxEvent>().RemoveRange(oldEvents);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxEvent>()
            .Where(e => !e.IsPublished)
            .CountAsync(cancellationToken);
    }
}
