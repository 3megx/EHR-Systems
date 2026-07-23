using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Billing.Domain;
using EHRPlatform.Services.Billing.Features.Claims.Commands;

namespace EHRPlatform.Services.Billing.Features.Claims.Handlers;

/// <summary>
/// Submit to insurance handler.
/// Pure business logic - no mapping responsibility.
/// </summary>
public class SubmitToInsuranceCommandHandler : ICommandHandler<SubmitToInsuranceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<SubmitToInsuranceCommandHandler> _logger;

    public SubmitToInsuranceCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<SubmitToInsuranceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(SubmitToInsuranceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting invoice {InvoiceId} to insurance {Provider}",
            command.InvoiceId, command.InsuranceProvider);

        var repo = _unitOfWork.Repository<Invoice>();
        var invoice = await repo.FirstOrDefaultAsync(
            q => q.Where(i => i.Id == command.InvoiceId),
            cancellationToken);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice {command.InvoiceId} not found");

        invoice.SubmitToInsurance(command.InsuranceProvider, command.PolicyNumber);
        await repo.UpdateAsync(invoice, cancellationToken);

        // Publish event
        var submitEvent = invoice.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = invoice.Id,
            EventType = nameof(InvoiceSubmittedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(submitEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Cancel invoice handler.
/// Pure business logic - no mapping responsibility.
/// </summary>
public class CancelInvoiceCommandHandler : ICommandHandler<CancelInvoiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<CancelInvoiceCommandHandler> _logger;

    public CancelInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<CancelInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(CancelInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling invoice {InvoiceId}", command.InvoiceId);

        var repo = _unitOfWork.Repository<Invoice>();
        var invoice = await repo.FirstOrDefaultAsync(
            q => q.Where(i => i.Id == command.InvoiceId),
            cancellationToken);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice {command.InvoiceId} not found");

        invoice.Cancel(command.Reason);
        await repo.UpdateAsync(invoice, cancellationToken);

        // Publish event
        var cancelEvent = invoice.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = invoice.Id,
            EventType = nameof(InvoiceCancelledEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(cancelEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
