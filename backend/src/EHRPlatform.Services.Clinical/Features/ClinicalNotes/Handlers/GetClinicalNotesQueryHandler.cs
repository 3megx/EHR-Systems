using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Queries;
using EHRPlatform.Services.Clinical.Application.ClinicalNotes.Responses;
using EHRPlatform.Services.Clinical.Application.ClinicalNotes.Mappers;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Handlers;

/// <summary>
/// Get clinical notes query handler.
/// Retrieves paginated list of clinical notes for patient.
/// </summary>
public class GetClinicalNotesQueryHandler : IQueryHandler<GetClinicalNotesQuery, ClinicalNoteListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ClinicalNoteMapper _mapper;
    private readonly ILogger<GetClinicalNotesQueryHandler> _logger;

    public GetClinicalNotesQueryHandler(
        IUnitOfWork unitOfWork,
        ClinicalNoteMapper mapper,
        ILogger<GetClinicalNotesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }

    public async Task<ClinicalNoteListDto> Handle(GetClinicalNotesQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving clinical notes for patient {PatientId}, page {PageNumber}", query.PatientId, query.PageNumber);

        var repo = _unitOfWork.Repository<Domain.Entities.ClinicalNote>();

        var (notes, total) = await repo.GetPagedAsync(
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            predicate: n => n.PatientId == query.PatientId && (query.Status == null || n.Status == query.Status),
            orderBy: n => n.OrderByDescending(x => x.EncounterDate),
            cancellationToken: cancellationToken
        );

        return _mapper.MapToListDto(notes, total, query.PageNumber, query.PageSize);
    }
}
