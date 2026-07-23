using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Handlers;

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
