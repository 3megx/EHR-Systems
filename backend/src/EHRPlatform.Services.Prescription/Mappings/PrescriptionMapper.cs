using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Domain;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Prescription.Mappings;

/// <summary>
/// Prescription Mapper
/// Single Responsibility: Convert between Prescription domain models and DTOs.
/// Handles all Prescription-related mappings with optional post-processing.
/// </summary>
public class PrescriptionMapper : MappingServiceBase<Prescription, PrescriptionResponseDto>
{
    public PrescriptionMapper(ILogger<PrescriptionMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single prescription to response DTO.
    /// </summary>
    public PrescriptionResponseDto MapToResponseDto(Prescription prescription)
    {
        return MapToDto(prescription);
    }

    /// <summary>
    /// Map prescription to detailed DTO with refills.
    /// </summary>
    public PrescriptionDetailedDto MapToDetailedDto(Prescription prescription)
    {
        Logger.LogDebug("Mapping prescription {PrescriptionId} to detailed DTO", prescription.Id);

        return new PrescriptionDetailedDto
        {
            Id = prescription.Id,
            PatientId = prescription.PatientId,
            ProviderId = prescription.ProviderId,
            MedicationName = prescription.MedicationName,
            Strength = prescription.Strength,
            FormType = prescription.FormType,
            Dosage = prescription.Dosage,
            Frequency = prescription.Frequency,
            Quantity = prescription.Quantity,
            RefillsAllowed = prescription.RefillsAllowed,
            RefillsUsed = prescription.RefillsUsed,
            StartDate = prescription.StartDate,
            EndDate = prescription.EndDate,
            Status = prescription.Status,
            Indications = prescription.Indications,
            SpecialInstructions = prescription.SpecialInstructions,
            PharmacyNotes = prescription.PharmacyNotes,
            IsControlledSubstance = prescription.IsControlledSubstance,
            NDCCode = prescription.NDCCode,
            Refills = prescription.Refills.Adapt<List<RefillDetailDto>>(),
            CanRefill = prescription.CanRefill(),
            CreatedAt = prescription.CreatedAt,
            LastModifiedAt = prescription.LastModifiedAt
        };
    }

    /// <summary>
    /// Map collection of prescriptions to paginated DTO.
    /// </summary>
    public PrescriptionListDto MapToListDto(
        ICollection<Prescription> prescriptions,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} prescriptions to paginated list DTO", prescriptions.Count);

        return new PrescriptionListDto
        {
            Items = prescriptions.Adapt<List<PrescriptionResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of prescriptions to response DTO list.
    /// </summary>
    public List<PrescriptionResponseDto> MapToResponseDtoList(ICollection<Prescription> prescriptions)
    {
        Logger.LogDebug("Mapping {Count} prescriptions to response DTO list", prescriptions.Count);
        return prescriptions.Adapt<List<PrescriptionResponseDto>>();
    }
}

/// <summary>
/// Prescription detailed DTO with relationships.
/// </summary>
public class PrescriptionDetailedDto
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
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Indications { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? PharmacyNotes { get; set; }
    public bool IsControlledSubstance { get; set; }
    public string? NDCCode { get; set; }
    public List<RefillDetailDto> Refills { get; set; } = new();
    public bool CanRefill { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

/// <summary>
/// Refill detail DTO.
/// </summary>
public class RefillDetailDto
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DeniedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PharmacyId { get; set; }
    public string? DenialReason { get; set; }
}

/// <summary>
/// Prescription list DTO with pagination.
/// </summary>
public class PrescriptionListDto
{
    public List<PrescriptionResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
