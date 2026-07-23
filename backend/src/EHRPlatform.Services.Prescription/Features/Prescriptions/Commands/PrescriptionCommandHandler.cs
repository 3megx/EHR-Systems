using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Domain;
using Mapster;

namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Commands;

/// <summary>
/// Issue prescription handler.
/// </summary>
public class IssuePrescriptionCommandHandler : ICommandHandler<IssuePrescriptionCommand, PrescriptionResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<IssuePrescriptionCommandHandler> _logger;

    public IssuePrescriptionCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<IssuePrescriptionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task<PrescriptionResponseDto> Handle(
        IssuePrescriptionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Issuing prescription: Patient {PatientId}, Provider {ProviderId}, Medication {Med}",
            command.PatientId, command.ProviderId, command.MedicationName);

        var prescription = new Domain.Prescription
        {
            Id = Guid.NewGuid(),
            PatientId = command.PatientId,
            ProviderId = command.ProviderId,
            MedicationName = command.MedicationName,
            Strength = command.Strength,
            FormType = command.FormType,
            Dosage = command.Dosage,
            Frequency = command.Frequency,
            Quantity = command.Quantity,
            RefillsAllowed = command.RefillsAllowed,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Indications = command.Indications,
            SpecialInstructions = command.SpecialInstructions,
            IsControlledSubstance = command.IsControlledSubstance,
            NDCCode = command.NDCCode
        };

        var repo = _unitOfWork.Repository<Domain.Prescription>();
        await repo.AddAsync(prescription, cancellationToken);

        // Publish event
        var issuedEvent = new PrescriptionIssuedEvent(
            prescription.Id, prescription.PatientId, prescription.ProviderId,
            prescription.MedicationName, prescription.Dosage);

        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = prescription.Id,
            EventType = nameof(PrescriptionIssuedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(issuedEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Prescription issued {PrescriptionId}", prescription.Id);

        return prescription.Adapt<PrescriptionResponseDto>();
    }
}

/// <summary>
/// Request refill handler.
/// </summary>
public class RequestRefillCommandHandler : ICommandHandler<RequestRefillCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<RequestRefillCommandHandler> _logger;

    public RequestRefillCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<RequestRefillCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(RequestRefillCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requesting refill for prescription {PrescriptionId}", command.PrescriptionId);

        var repo = _unitOfWork.Repository<Domain.Prescription>();
        var prescription = await repo.FirstOrDefaultAsync(
            q => q.Where(p => p.Id == command.PrescriptionId),
            cancellationToken);

        if (prescription == null)
            throw new InvalidOperationException($"Prescription {command.PrescriptionId} not found");

        prescription.RequestRefill(command.PharmacyId ?? "");
        await repo.UpdateAsync(prescription, cancellationToken);

        // Publish event
        var refillEvent = prescription.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = prescription.Id,
            EventType = nameof(PrescriptionRefillRequestedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(refillEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Approve refill handler.
/// </summary>
public class ApproveRefillCommandHandler : ICommandHandler<ApproveRefillCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<ApproveRefillCommandHandler> _logger;

    public ApproveRefillCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<ApproveRefillCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(ApproveRefillCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving refill {RefillId} for prescription {PrescriptionId}",
            command.RefillId, command.PrescriptionId);

        var repo = _unitOfWork.Repository<Domain.Prescription>();
        var prescription = await repo.FirstOrDefaultAsync(
            q => q.Where(p => p.Id == command.PrescriptionId),
            cancellationToken);

        if (prescription == null)
            throw new InvalidOperationException($"Prescription {command.PrescriptionId} not found");

        prescription.ApproveRefill(command.RefillId);
        await repo.UpdateAsync(prescription, cancellationToken);

        // Publish event
        var approveEvent = prescription.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = prescription.Id,
            EventType = nameof(PrescriptionRefillApprovedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(approveEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Suspend prescription handler.
/// </summary>
public class SuspendPrescriptionCommandHandler : ICommandHandler<SuspendPrescriptionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<SuspendPrescriptionCommandHandler> _logger;

    public SuspendPrescriptionCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<SuspendPrescriptionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(SuspendPrescriptionCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Suspending prescription {PrescriptionId}", command.PrescriptionId);

        var repo = _unitOfWork.Repository<Domain.Prescription>();
        var prescription = await repo.FirstOrDefaultAsync(
            q => q.Where(p => p.Id == command.PrescriptionId),
            cancellationToken);

        if (prescription == null)
            throw new InvalidOperationException($"Prescription {command.PrescriptionId} not found");

        prescription.Suspend(command.Reason);
        await repo.UpdateAsync(prescription, cancellationToken);

        // Publish event
        var suspendEvent = prescription.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = prescription.Id,
            EventType = nameof(PrescriptionSuspendedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(suspendEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Resume prescription handler.
/// </summary>
public class ResumePrescriptionCommandHandler : ICommandHandler<ResumePrescriptionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<ResumePrescriptionCommandHandler> _logger;

    public ResumePrescriptionCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<ResumePrescriptionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(ResumePrescriptionCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resuming prescription {PrescriptionId}", command.PrescriptionId);

        var repo = _unitOfWork.Repository<Domain.Prescription>();
        var prescription = await repo.FirstOrDefaultAsync(
            q => q.Where(p => p.Id == command.PrescriptionId),
            cancellationToken);

        if (prescription == null)
            throw new InvalidOperationException($"Prescription {command.PrescriptionId} not found");

        prescription.Resume();
        await repo.UpdateAsync(prescription, cancellationToken);

        // Publish event
        var resumeEvent = prescription.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = prescription.Id,
            EventType = nameof(PrescriptionResumedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(resumeEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Discontinue prescription handler.
/// </summary>
public class DiscontinuePrescriptionCommandHandler : ICommandHandler<DiscontinuePrescriptionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<DiscontinuePrescriptionCommandHandler> _logger;

    public DiscontinuePrescriptionCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<DiscontinuePrescriptionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(DiscontinuePrescriptionCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Discontinuing prescription {PrescriptionId}", command.PrescriptionId);

        var repo = _unitOfWork.Repository<Domain.Prescription>();
        var prescription = await repo.FirstOrDefaultAsync(
            q => q.Where(p => p.Id == command.PrescriptionId),
            cancellationToken);

        if (prescription == null)
            throw new InvalidOperationException($"Prescription {command.PrescriptionId} not found");

        prescription.Discontinue(command.Reason);
        await repo.UpdateAsync(prescription, cancellationToken);

        // Publish event
        var discontinueEvent = prescription.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = prescription.Id,
            EventType = nameof(PrescriptionDiscontinuedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(discontinueEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
