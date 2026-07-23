using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Clinical.Mappings;

/// <summary>
/// Clinical Mapper
/// Single Responsibility: Convert between Clinical domain models and DTOs.
/// Handles all Clinical-related mappings with optional post-processing.
/// </summary>
public class ClinicalMapper : MappingServiceBase<ClinicalNote, ClinicalNoteResponseDto>
{
    public ClinicalMapper(ILogger<ClinicalMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single clinical note to response DTO.
    /// </summary>
    public ClinicalNoteResponseDto MapToResponseDto(ClinicalNote clinicalNote)
    {
        return MapToDto(clinicalNote);
    }

    /// <summary>
    /// Map clinical note to detailed DTO with vitals, diagnoses, and procedures.
    /// </summary>
    public ClinicalNoteDetailedDto MapToDetailedDto(ClinicalNote clinicalNote)
    {
        Logger.LogDebug("Mapping clinical note {ClinicalNoteId} to detailed DTO", clinicalNote.Id);

        return new ClinicalNoteDetailedDto
        {
            Id = clinicalNote.Id,
            PatientId = clinicalNote.PatientId,
            ProviderId = clinicalNote.ProviderId,
            EncounterDate = clinicalNote.EncounterDate,
            EncounterType = clinicalNote.EncounterType,
            Status = clinicalNote.Status,
            Subjective = clinicalNote.Subjective,
            Objective = clinicalNote.Objective,
            Assessment = clinicalNote.Assessment,
            Plan = clinicalNote.Plan,
            VitalSigns = clinicalNote.VitalSigns.Adapt<List<VitalSignsDetailDto>>(),
            Diagnoses = clinicalNote.Diagnoses.Adapt<List<DiagnosisDetailDto>>(),
            Procedures = clinicalNote.Procedures.Adapt<List<ProcedureDetailDto>>(),
            CreatedAt = clinicalNote.CreatedAt,
            LastModifiedAt = clinicalNote.LastModifiedAt
        };
    }

    /// <summary>
    /// Map collection of clinical notes to paginated DTO.
    /// </summary>
    public ClinicalNoteListDto MapToListDto(
        ICollection<ClinicalNote> clinicalNotes,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} clinical notes to paginated list DTO", clinicalNotes.Count);

        return new ClinicalNoteListDto
        {
            Items = clinicalNotes.Adapt<List<ClinicalNoteResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of clinical notes to response DTO list.
    /// </summary>
    public List<ClinicalNoteResponseDto> MapToResponseDtoList(ICollection<ClinicalNote> clinicalNotes)
    {
        Logger.LogDebug("Mapping {Count} clinical notes to response DTO list", clinicalNotes.Count);
        return clinicalNotes.Adapt<List<ClinicalNoteResponseDto>>();
    }
}

/// <summary>
/// Clinical note detailed DTO with relationships.
/// </summary>
public class ClinicalNoteDetailedDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public List<VitalSignsDetailDto> VitalSigns { get; set; } = new();
    public List<DiagnosisDetailDto> Diagnoses { get; set; } = new();
    public List<ProcedureDetailDto> Procedures { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
