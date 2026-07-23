using AutoMapper;
using EHRPlatform.Services.Audit.Domain.Entities;
using EHRPlatform.Services.Audit.Application.Audit.Responses;

namespace EHRPlatform.Services.Audit.Application.Audit.Mappers;

/// <summary>
/// Mapper for Audit DTOs.
/// </summary>
public class AuditMapper
{
    private readonly IMapper _mapper;

    public AuditMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public AuditEntryResponse MapToAuditEntryResponse(AuditEntry auditEntry)
    {
        return _mapper.Map<AuditEntryResponse>(auditEntry);
    }
}
