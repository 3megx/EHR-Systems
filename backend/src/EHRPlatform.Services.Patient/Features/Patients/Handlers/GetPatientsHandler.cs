using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Patient.Features.Patients.Queries;
using EHRPlatform.Services.Patient.Application.Patients.Responses;
using EHRPlatform.Services.Patient.Application.Patients.Mappers;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Patient.Features.Patients.Handlers;

/// <summary>
/// Get patients query handler.
/// Retrieves paginated list of patients with optional filtering.
/// </summary>
public class GetPatientsQueryHandler : IQueryHandler<GetPatientsQuery, PatientListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PatientMapper _mapper;
    private readonly ILogger<GetPatientsQueryHandler> _logger;

    public GetPatientsQueryHandler(
        IUnitOfWork unitOfWork,
        PatientMapper mapper,
        ILogger<GetPatientsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }

    public async Task<PatientListDto> Handle(GetPatientsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving patients, page {PageNumber}, status filter: {Status}", query.PageNumber, query.Status);

        var repo = _unitOfWork.Repository<Domain.Entities.Patient>();
        
        // Build query with filters
        var (patients, total) = await repo.GetPagedAsync(
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            predicate: p => query.Status == null || p.Status == query.Status,
            orderBy: p => p.OrderByDescending(x => x.CreatedAt),
            cancellationToken: cancellationToken
        );

        return _mapper.MapToListDto(patients, total, query.PageNumber, query.PageSize);
    }
}
