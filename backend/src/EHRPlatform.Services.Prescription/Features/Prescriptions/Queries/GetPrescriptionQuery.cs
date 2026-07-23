using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Queries;

/// <summary>
/// Get prescription by ID - CACHED query.
/// </summary>
public record GetPrescriptionQuery : ICachedQuery<PrescriptionResponseDto>
{
    public Guid PrescriptionId { get; init; }

    public string CacheKey => $"prescription_{PrescriptionId}";
    public int CacheDurationSeconds => 600; // 10 minutes
}

/// <summary>
/// Get patient active prescriptions - CACHED query.
/// </summary>
public record GetPatientActivePrescriptionsQuery : ICachedQuery<PrescriptionListDto>
{
    public Guid PatientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"prescriptions_patient_{PatientId}_active_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get patient all prescriptions history - CACHED query.
/// </summary>
public record GetPatientPrescriptionHistoryQuery : ICachedQuery<PrescriptionListDto>
{
    public Guid PatientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"prescriptions_patient_{PatientId}_history_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get pending refill requests for provider - CACHED query.
/// </summary>
public record GetPendingRefillsQuery : ICachedQuery<RefillRequestListDto>
{
    public Guid ProviderId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"refills_provider_{ProviderId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 300; // 5 minutes - more frequently updated
}

/// <summary>
/// Prescription list DTO.
/// </summary>
public class PrescriptionListDto
{
    public List<PrescriptionResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Prescription response DTO.
/// </summary>
public class PrescriptionResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public string FormType { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int RefillsAllowed { get; set; }
    public int RefillsUsed { get; set; }
    public int RefillsRemaining => RefillsAllowed - RefillsUsed;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Indications { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool IsControlledSubstance { get; set; }
    public string? NDCCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RefillDto> Refills { get; set; } = new();
}

public class RefillDto
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PharmacyId { get; set; }
}

/// <summary>
/// Refill request list DTO.
/// </summary>
public class RefillRequestListDto
{
    public List<RefillRequestDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class RefillRequestDto
{
    public Guid RefillId { get; set; }
    public Guid PrescriptionId { get; set; }
    public Guid PatientId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PharmacyId { get; set; }
}
