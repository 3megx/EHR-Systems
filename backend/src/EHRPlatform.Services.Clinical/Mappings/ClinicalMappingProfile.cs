using Mapster;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Queries;

namespace EHRPlatform.Services.Clinical.Mappings;

/// <summary>
/// Mapster registration profile for Clinical entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Clinical-related type mappings.
/// </summary>
public class ClinicalMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ClinicalNote → ClinicalNoteResponseDto
        config.NewConfig<ClinicalNote, ClinicalNoteResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.ProviderId, src => src.ProviderId)
            .Map(dest => dest.EncounterDate, src => src.EncounterDate)
            .Map(dest => dest.EncounterType, src => src.EncounterType)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Subjective, src => src.Subjective)
            .Map(dest => dest.Objective, src => src.Objective)
            .Map(dest => dest.Assessment, src => src.Assessment)
            .Map(dest => dest.Plan, src => src.Plan)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.LastModifiedAt, src => src.LastModifiedAt);

        // VitalSigns → VitalSignsDetailDto
        config.NewConfig<VitalSigns, VitalSignsDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.RecordedAt, src => src.RecordedAt)
            .Map(dest => dest.Temperature, src => src.Temperature)
            .Map(dest => dest.SystolicBP, src => src.SystolicBP)
            .Map(dest => dest.DiastolicBP, src => src.DiastolicBP)
            .Map(dest => dest.HeartRate, src => src.HeartRate)
            .Map(dest => dest.RespiratoryRate, src => src.RespiratoryRate)
            .Map(dest => dest.Weight, src => src.Weight)
            .Map(dest => dest.BloodPressure, src => src.GetBloodPressure());

        // ClinicalDiagnosis → DiagnosisDetailDto
        config.NewConfig<ClinicalDiagnosis, DiagnosisDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.DiagnosisCode, src => src.DiagnosisCode)
            .Map(dest => dest.DiagnosisText, src => src.DiagnosisText)
            .Map(dest => dest.DiagnosisType, src => src.DiagnosisType)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // ClinicalProcedure → ProcedureDetailDto
        config.NewConfig<ClinicalProcedure, ProcedureDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ProcedureName, src => src.ProcedureName)
            .Map(dest => dest.ProcedureCode, src => src.ProcedureCode)
            .Map(dest => dest.PerformedAt, src => src.PerformedAt)
            .Map(dest => dest.Result, src => src.Result)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // ClinicalNoteResponseDto → ClinicalNote (for updates)
        config.NewConfig<ClinicalNoteResponseDto, ClinicalNote>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.ProviderId, src => src.ProviderId)
            .Map(dest => dest.EncounterDate, src => src.EncounterDate)
            .Map(dest => dest.EncounterType, src => src.EncounterType)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Subjective, src => src.Subjective)
            .Map(dest => dest.Objective, src => src.Objective)
            .Map(dest => dest.Assessment, src => src.Assessment)
            .Map(dest => dest.Plan, src => src.Plan);
    }
}

/// <summary>
/// Clinical note response DTO.
/// </summary>
public class ClinicalNoteResponseDto
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
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

/// <summary>
/// Vital signs detail DTO.
/// </summary>
public class VitalSignsDetailDto
{
    public Guid Id { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Temperature { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
}

/// <summary>
/// Diagnosis detail DTO.
/// </summary>
public class DiagnosisDetailDto
{
    public Guid Id { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Procedure detail DTO.
/// </summary>
public class ProcedureDetailDto
{
    public Guid Id { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Clinical note list DTO with pagination.
/// </summary>
public class ClinicalNoteListDto
{
    public List<ClinicalNoteResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
