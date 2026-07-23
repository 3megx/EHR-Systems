using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Billing.Features.Billing.Domain;
using EHRPlatform.Services.Billing.Features.Billing.Dtos.Responses;
using EHRPlatform.Services.Billing.Mappings;

namespace EHRPlatform.Services.Billing.Features.Billing.Commands;

/// <summary>
/// Create invoice handler.
/// Pure business logic - no mapping responsibility.
/// </summary>
public class CreateInvoiceCommandHandler : ICommandHandler<CreateInvoiceCommand, InvoiceResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly InvoiceMapper _mapper;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    public CreateInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        InvoiceMapper mapper,
        ILogger<CreateInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }

    public async Task<InvoiceResponseDto> Handle(
        CreateInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating invoice for patient {PatientId}", command.PatientId);

        var invoiceNumber = GenerateInvoiceNumber();
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            PatientId = command.PatientId,
            AppointmentId = command.AppointmentId,
            InvoiceNumber = invoiceNumber,
            ServiceDate = command.ServiceDate,
            DueDate = command.ServiceDate.AddDays(30), // 30-day payment terms
            Status = "Draft",
            InsuranceProvider = command.InsuranceProvider,
            InsurancePolicyNumber = command.InsurancePolicyNumber,
            Notes = command.Notes
        };

        // Add line items
        foreach (var item in command.LineItems)
        {
            invoice.AddLineItem(item.Description, item.CPTCode, item.Quantity, item.UnitPrice);
        }

        // Calculate totals
        invoice.CalculateTotals();

        var repo = _unitOfWork.Repository<Invoice>();
        await repo.AddAsync(invoice, cancellationToken);

        // Publish event
        var createdEvent = new InvoiceCreatedEvent(
            invoice.Id, invoice.PatientId, invoice.TotalAmount, invoiceNumber);

        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = invoice.Id,
            EventType = nameof(InvoiceCreatedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(createdEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invoice created {InvoiceId} (#{Number})", invoice.Id, invoiceNumber);

        // Delegate mapping to mapper
        return _mapper.MapToResponseDto(invoice);
    }

    private static string GenerateInvoiceNumber()
    {
        // Format: INV-YYYYMMDD-XXXXXX
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(100000, 999999);
        return $"INV-{timestamp}-{random}";
    }
}

/// <summary>
/// Record payment handler.
/// Pure business logic - no mapping responsibility.
/// </summary>
public class RecordPaymentCommandHandler : ICommandHandler<RecordPaymentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<RecordPaymentCommandHandler> _logger;

    public RecordPaymentCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<RecordPaymentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording payment {Amount} for invoice {InvoiceId}",
            command.Amount, command.InvoiceId);

        var repo = _unitOfWork.Repository<Invoice>();
        var invoice = await repo.FirstOrDefaultAsync(
            q => q.Where(i => i.Id == command.InvoiceId),
            cancellationToken);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice {command.InvoiceId} not found");

        invoice.RecordPayment(command.Amount, command.Method, command.Reference ?? "");

        await repo.UpdateAsync(invoice, cancellationToken);

        // Publish event
        var paymentEvent = invoice.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = invoice.Id,
            EventType = nameof(PaymentReceivedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(paymentEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

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
