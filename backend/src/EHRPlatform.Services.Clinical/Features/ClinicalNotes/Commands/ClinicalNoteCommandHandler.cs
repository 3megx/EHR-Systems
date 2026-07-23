using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;
using Mapster;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;

/// <summary>
/// Create clinical note handler.
/// </summary>
public class CreateClinicalNoteCommandHandler : ICommandHandler<CreateClinicalNoteCommand, ClinicalNoteResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<CreateClinicalNoteCommandHandler> _logger;

    public CreateClinicalNoteCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<CreateClinicalNoteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task<ClinicalNoteResponseDto> Handle(
        CreateClinicalNoteCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating clinical note for patient {PatientId}", command.PatientId);

        var note = new ClinicalNote
        {
            Id = Guid.NewGuid(),
            PatientId = command.PatientId,
            ProviderId = command.ProviderId,
            EncounterDate = command.EncounterDate,
            EncounterType = command.EncounterType,
            Status = "Draft"
        };

        var repo = _unitOfWork.Repository<ClinicalNote>();
        await repo.AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Clinical note created {ClinicalNoteId}", note.Id);

        return note.Adapt<ClinicalNoteResponseDto>();
    }
}

/// <summary>
/// Add diagnosis handler.
/// </summary>
public class AddDiagnosisCommandHandler : ICommandHandler<AddDiagnosisCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<AddDiagnosisCommandHandler> _logger;

    public AddDiagnosisCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<AddDiagnosisCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(AddDiagnosisCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding diagnosis {Code} to note {NoteId}", command.DiagnosisCode, command.ClinicalNoteId);

        var repo = _unitOfWork.Repository<ClinicalNote>();
        var note = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.ClinicalNoteId),
            cancellationToken);

        if (note == null)
            throw new InvalidOperationException($"Clinical note {command.ClinicalNoteId} not found");

        note.AddDiagnosis(command.DiagnosisCode, command.DiagnosisText, command.DiagnosisType);

        await repo.UpdateAsync(note, cancellationToken);

        // Publish event
        var diagEvent = note.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = note.Id,
            EventType = nameof(DiagnosisRecordedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(diagEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Record vitals handler.
/// </summary>
public class RecordVitalsCommandHandler : ICommandHandler<RecordVitalsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<RecordVitalsCommandHandler> _logger;

    public RecordVitalsCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<RecordVitalsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(RecordVitalsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording vitals for note {NoteId}", command.ClinicalNoteId);

        var repo = _unitOfWork.Repository<ClinicalNote>();
        var note = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.ClinicalNoteId),
            cancellationToken);

        if (note == null)
            throw new InvalidOperationException($"Clinical note {command.ClinicalNoteId} not found");

        note.RecordVitals(
            command.Temperature,
            command.SystolicBP,
            command.DiastolicBP,
            command.HeartRate,
            command.RespiratoryRate,
            command.Weight);

        await repo.UpdateAsync(note, cancellationToken);

        // Publish event
        var vitalsEvent = note.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = note.Id,
            EventType = nameof(VitalSignsRecordedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(vitalsEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Add procedure handler.
/// </summary>
public class AddProcedureCommandHandler : ICommandHandler<AddProcedureCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<AddProcedureCommandHandler> _logger;

    public AddProcedureCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<AddProcedureCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(AddProcedureCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding procedure to note {NoteId}", command.ClinicalNoteId);

        var repo = _unitOfWork.Repository<ClinicalNote>();
        var note = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.ClinicalNoteId),
            cancellationToken);

        if (note == null)
            throw new InvalidOperationException($"Clinical note {command.ClinicalNoteId} not found");

        note.AddProcedure(command.ProcedureName, command.ProcedureCode, command.Result);

        await repo.UpdateAsync(note, cancellationToken);

        // Publish event
        var procEvent = note.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = note.Id,
            EventType = nameof(ProcedurePerformedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(procEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Update SOAP note handler.
/// </summary>
public class UpdateSOAPCommandHandler : ICommandHandler<UpdateSOAPCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSOAPCommandHandler> _logger;

    public UpdateSOAPCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSOAPCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateSOAPCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating SOAP note {NoteId}", command.ClinicalNoteId);

        var repo = _unitOfWork.Repository<ClinicalNote>();
        var note = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.ClinicalNoteId),
            cancellationToken);

        if (note == null)
            throw new InvalidOperationException($"Clinical note {command.ClinicalNoteId} not found");

        if (note.Status != "Draft")
            throw new InvalidOperationException("Only draft notes can be edited");

        if (!string.IsNullOrEmpty(command.Subjective))
            note.Subjective = command.Subjective;
        if (!string.IsNullOrEmpty(command.Objective))
            note.Objective = command.Objective;
        if (!string.IsNullOrEmpty(command.Assessment))
            note.Assessment = command.Assessment;
        if (!string.IsNullOrEmpty(command.Plan))
            note.Plan = command.Plan;

        await repo.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Finalize clinical note handler.
/// </summary>
public class FinalizeClinicalNoteCommandHandler : ICommandHandler<FinalizeClinicalNoteCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<FinalizeClinicalNoteCommandHandler> _logger;

    public FinalizeClinicalNoteCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<FinalizeClinicalNoteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(FinalizeClinicalNoteCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Finalizing clinical note {NoteId}", command.ClinicalNoteId);

        var repo = _unitOfWork.Repository<ClinicalNote>();
        var note = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.ClinicalNoteId),
            cancellationToken);

        if (note == null)
            throw new InvalidOperationException($"Clinical note {command.ClinicalNoteId} not found");

        note.Finalize();

        await repo.UpdateAsync(note, cancellationToken);

        // Publish completion event
        var completeEvent = note.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = note.Id,
            EventType = nameof(ClinicalNoteCompletedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(completeEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
