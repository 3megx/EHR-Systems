using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;

/// <summary>
/// Finalize clinical note command.
/// Locks note for editing, publishes event.
/// </summary>
public record FinalizeClinicalNoteCommand : ICommand
{
    public Guid ClinicalNoteId { get; init; }
}
