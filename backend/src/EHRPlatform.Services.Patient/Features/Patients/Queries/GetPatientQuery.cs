using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Patient.Features.Patients.Queries;

/// <summary>
/// Get patient by ID - CACHED query.
/// Automatically cached for 15 minutes.
/// </summary>
public record GetPatientQuery : ICachedQuery<PatientResponseDto>
{
    public Guid PatientId { get; init; }

    public string CacheKey => $"patient_{PatientId}";
    public int CacheDurationSeconds => 900; // 15 minutes
}

/// <summary>
/// Search patients with pagination.
/// Full-text search with Elasticsearch.
/// </summary>
public record SearchPatientsQuery : ICachedQuery<SearchResultDto<PatientResponseDto>>
{
    public string SearchTerm { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public string CacheKey => $"patients_search_{SearchTerm}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600; // 10 minutes
}

/// <summary>
/// List all patients with pagination.
/// Cached query.
/// </summary>
public record ListPatientsQuery : ICachedQuery<SearchResultDto<PatientResponseDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"patients_list_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get patient with full details including allergies and conditions.
/// </summary>
public record GetPatientDetailQuery : ICachedQuery<PatientDetailDto>
{
    public Guid PatientId { get; init; }

    public string CacheKey => $"patient_detail_{PatientId}";
    public int CacheDurationSeconds => 900;
}

/// <summary>
/// Patient detail response with relationships.
/// </summary>
public class PatientDetailDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<AllergyDetailDto> Allergies { get; set; } = new();
    public List<ConditionDetailDto> Conditions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class AllergyDetailDto
{
    public Guid Id { get; set; }
    public string Allergen { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ConditionDetailDto
{
    public Guid Id { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string ICD10Code { get; set; } = string.Empty;
    public DateTime? OnsetDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Search result wrapper with pagination.
/// </summary>
public class SearchResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}
