using Mapster;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Domain;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Queries;

namespace EHRPlatform.Services.Prescription.Mappings;

/// <summary>
/// Mapster registration profile for Prescription entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Prescription-related type mappings.
/// </summary>
public class PrescriptionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Prescription → PrescriptionResponseDto
        config.NewConfig<Prescription, PrescriptionResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.ProviderId, src => src.ProviderId)
            .Map(dest => dest.MedicationName, src => src.MedicationName)
            .Map(dest => dest.Strength, src => src.Strength)
            .Map(dest => dest.FormType, src => src.FormType)
            .Map(dest => dest.Dosage, src => src.Dosage)
            .Map(dest => dest.Frequency, src => src.Frequency)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.RefillsAllowed, src => src.RefillsAllowed)
            .Map(dest => dest.RefillsUsed, src => src.RefillsUsed)
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Indications, src => src.Indications)
            .Map(dest => dest.SpecialInstructions, src => src.SpecialInstructions)
            .Map(dest => dest.IsControlledSubstance, src => src.IsControlledSubstance)
            .Map(dest => dest.NDCCode, src => src.NDCCode)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.LastModifiedAt, src => src.LastModifiedAt);

        // PrescriptionRefill → RefillDetailDto
        config.NewConfig<PrescriptionRefill, RefillDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.RequestedAt, src => src.RequestedAt)
            .Map(dest => dest.ApprovedAt, src => src.ApprovedAt)
            .Map(dest => dest.DeniedAt, src => src.DeniedAt)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.PharmacyId, src => src.PharmacyId)
            .Map(dest => dest.DenialReason, src => src.DenialReason);

        // PrescriptionResponseDto → Prescription (for updates)
        config.NewConfig<PrescriptionResponseDto, Prescription>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.ProviderId, src => src.ProviderId)
            .Map(dest => dest.MedicationName, src => src.MedicationName)
            .Map(dest => dest.Strength, src => src.Strength)
            .Map(dest => dest.FormType, src => src.FormType)
            .Map(dest => dest.Dosage, src => src.Dosage)
            .Map(dest => dest.Frequency, src => src.Frequency)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.RefillsAllowed, src => src.RefillsAllowed)
            .Map(dest => dest.RefillsUsed, src => src.RefillsUsed)
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Indications, src => src.Indications)
            .Map(dest => dest.SpecialInstructions, src => src.SpecialInstructions)
            .Map(dest => dest.IsControlledSubstance, src => src.IsControlledSubstance)
            .Map(dest => dest.NDCCode, src => src.NDCCode);
    }
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
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Indications { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool IsControlledSubstance { get; set; }
    public string? NDCCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
