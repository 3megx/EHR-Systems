using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;
using EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Responses;
using Mapster;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Handlers;

/// <summary>
/// Create clinical note handler.
/// </summary>
public class CreateClinicalNoteCommandHandler : ICommandHandler<CreateClinicalNoteCommand, ClinicalNoteResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClinicalNoteCommandHandler> _logger;

    public CreateClinicalNoteCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateClinicalNoteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
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
