using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Queries;

/// <summary>
/// Get clinical note by ID - CACHED query.
/// </summary>
public record GetClinicalNoteQuery : ICachedQuery<ClinicalNoteResponseDto>
{
    public Guid ClinicalNoteId { get; init; }

    public string CacheKey => $"clinical_note_{ClinicalNoteId}";
    public int CacheDurationSeconds => 900; // 15 minutes
}

/// <summary>
/// Get patient clinical timeline.
/// All clinical notes with vitals and diagnoses.
/// CACHED query.
/// </summary>
public record GetPatientClinicalTimelineQuery : ICachedQuery<ClinicalTimelineDto>
{
    public Guid PatientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public string CacheKey => $"clinical_timeline_{PatientId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600; // 10 minutes
}

/// <summary>
/// Get patient vital signs timeline.
/// Time-series vital records.
/// CACHED query.
/// </summary>
public record GetVitalSignsTimelineQuery : ICachedQuery<VitalSignsTimelineDto>
{
    public Guid PatientId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    public string CacheKey => $"vitals_timeline_{PatientId}_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get patient diagnoses history.
/// All ICD-10 diagnoses with dates.
/// CACHED query.
/// </summary>
public record GetDiagnosisHistoryQuery : ICachedQuery<DiagnosisHistoryDto>
{
    public Guid PatientId { get; init; }

    public string CacheKey => $"diagnosis_history_{PatientId}";
    public int CacheDurationSeconds => 900;
}

/// <summary>
/// Clinical timeline response.
/// </summary>
public class ClinicalTimelineDto
{
    public Guid PatientId { get; set; }
    public List<ClinicalNoteTimelineItemDto> Notes { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class ClinicalNoteTimelineItemDto
{
    public Guid Id { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid ProviderId { get; set; }
    public List<DiagnosisDto> Diagnoses { get; set; } = new();
    public VitalSignsDto? LatestVitals { get; set; }
}

/// <summary>
/// Vital signs timeline response.
/// </summary>
public class VitalSignsTimelineDto
{
    public Guid PatientId { get; set; }
    public List<VitalSignsRecordDto> Records { get; set; } = new();
    public VitalSignsStatisticsDto Statistics { get; set; } = new();
}

public class VitalSignsRecordDto
{
    public Guid Id { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Temperature { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
}

public class VitalSignsStatisticsDto
{
    public decimal AverageTemperature { get; set; }
    public int AverageSystolicBP { get; set; }
    public int AverageDiastolicBP { get; set; }
    public int AverageHeartRate { get; set; }
}

/// <summary>
/// Diagnosis history response.
/// </summary>
public class DiagnosisHistoryDto
{
    public Guid PatientId { get; set; }
    public List<DiagnosisHistoryItemDto> Diagnoses { get; set; } = new();
}

public class DiagnosisHistoryItemDto
{
    public Guid Id { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty;
    public DateTime RecordedDate { get; set; }
    public Guid ClinicalNoteId { get; set; }
}

/// <summary>
/// Common diagnosis DTO.
/// </summary>
public class DiagnosisDto
{
    public Guid Id { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty;
}
