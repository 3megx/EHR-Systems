using AutoMapper;
using EHRPlatform.Services.Audit.Domain.Entities;
using EHRPlatform.Services.Audit.Application.Audit.Responses;

namespace EHRPlatform.Services.Audit.Application.Audit.Mappers;

/// <summary>
/// AutoMapper profile for Audit entities.
/// </summary>
public class AuditMappingProfile : Profile
{
    public AuditMappingProfile()
    {
        CreateMap<AuditEntry, AuditEntryResponse>();
        CreateMap<AccessLog, AccessLogResponse>();
    }
}

public class AccessLogResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public DateTime AccessedAt { get; set; }
}
