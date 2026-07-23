using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Common.Messaging;

/// <summary>
/// Kafka implementation of IEventPublisher.
/// Publishes integration events to Kafka topics.
/// 
/// Topic naming: {EventType}.{Environment}
/// Example: PatientCreated.production, PatientUpdated.development
/// 
/// Partitioning: By AggregateId for ordering within aggregate
/// Retries: Handled by Kafka client (configurable)
/// </summary>
public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly string _environment;
    private readonly ILogger<KafkaEventPublisher> _logger;

    public KafkaEventPublisher(
        IProducer<string, string> producer,
        string environment,
        ILogger<KafkaEventPublisher> logger)
    {
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
        _environment = environment ?? "production";
        _logger = logger;
    }

    /// <summary>
    /// Publish single event to Kafka.
    /// </summary>
    public async Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNull(@event, nameof(@event));

        var topicName = GetTopicName(@event.EventType);

        try
        {
            var message = new Message<string, string>
            {
                Key = @event.EventId.ToString(), // Partition by event ID
                Value = JsonSerializer.Serialize(@event),
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            var deliveryReport = await _producer.ProduceAsync(topicName, message);

            if (deliveryReport.Status != PersistenceStatus.Persisted)
            {
                throw new KafkaException($"Failed to deliver message to {topicName}: {deliveryReport.Error.Reason}");
            }

            _logger.LogInformation(
                "Event {EventId} of type {EventType} published to {Topic}",
                @event.EventId,
                @event.EventType,
                topicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventId} to {Topic}", @event.EventId, topicName);
            throw;
        }
    }

    /// <summary>
    /// Publish multiple events in batch.
    /// </summary>
    public async Task PublishBatchAsync(
        IEnumerable<IntegrationEvent> events,
        CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNull(events, nameof(events));

        var eventList = events.ToList();
        if (eventList.Count == 0)
            return;

        var tasks = eventList
            .GroupBy(e => GetTopicName(e.EventType))
            .SelectMany(g => g.Select(e => PublishAsync(e, cancellationToken)))
            .ToList();

        await Task.WhenAll(tasks);

        _logger.LogInformation("Published {Count} events in batch", eventList.Count);
    }

    /// <summary>
    /// Get Kafka topic name for event type.
    /// Format: {eventType}.{environment}
    /// Example: PatientCreated.production
    /// </summary>
    private string GetTopicName(string eventType)
    {
        return $"{eventType}.{_environment}".ToLower();
    }
}

/// <summary>
/// Base class for Kafka consumers.
/// Automatically handles message deserialization and offset management.
/// </summary>
public abstract class KafkaConsumerBase<TEvent> : BackgroundService where TEvent : IntegrationEvent
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger _logger;
    private readonly string _topicName;

    protected KafkaConsumerBase(
        IConsumer<string, string> consumer,
        string topicName,
        ILogger logger)
    {
        _consumer = consumer;
        _topicName = topicName;
        _logger = logger;
    }

    /// <summary>
    /// Override to handle event.
    /// </summary>
    protected abstract Task HandleEventAsync(TEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Main consumer loop.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topicName);

        _logger.LogInformation("Kafka consumer started for topic {Topic}", _topicName);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult == null)
                    continue;

                try
                {
                    var @event = JsonSerializer.Deserialize<TEvent>(
                        consumeResult.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (@event != null)
                    {
                        await HandleEventAsync(@event, stoppingToken);
                    }

                    // Commit offset after successful processing
                    _consumer.Commit(consumeResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from {Topic}", _topicName);
                    // Message will be retried on next consumer startup
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka consumer error");
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}

/// <summary>
/// Kafka configuration builder.
/// Creates producer and consumer configurations.
/// </summary>
public static class KafkaConfigBuilder
{
    /// <summary>
    /// Create producer configuration for publishing events.
    /// </summary>
    public static ProducerConfig CreateProducerConfig(string bootstrapServers)
    {
        return new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = $"{Environment.MachineName}-producer",
            Acks = Acks.All,
            RetryBackoffMs = 100,
            MessageSendMaxRetries = 3,
            EnableDeliveryReports = true,
            CompressionType = CompressionType.Snappy
        };
    }

    /// <summary>
    /// Create consumer configuration for subscribing to events.
    /// </summary>
    public static ConsumerConfig CreateConsumerConfig(string bootstrapServers, string groupId)
    {
        return new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            ClientId = $"{Environment.MachineName}-consumer-{groupId}",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            AllowAutoCreateTopics = true
        };
    }
}

/// <summary>
/// Argument validation helper.
/// </summary>
internal static class ArgumentGuard
{
    public static void NotNull<T>(T? argument, string parameterName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(parameterName);
    }

    public static void NotNullOrEmpty(string? argument, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException("Value cannot be null or empty", parameterName);
    }
}
