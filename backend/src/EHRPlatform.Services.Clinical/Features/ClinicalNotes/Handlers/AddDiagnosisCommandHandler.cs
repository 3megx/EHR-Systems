using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Handlers;

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
