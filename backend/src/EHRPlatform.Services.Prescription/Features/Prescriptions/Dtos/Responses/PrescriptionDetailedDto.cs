namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Dtos.Responses;

/// <summary>
/// Detailed prescription DTO.
/// Single Responsibility: Represent complete prescription with all details and refill history.
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
    public int RefillsRemaining => RefillsAllowed - RefillsUsed;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Indications { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool IsControlledSubstance { get; set; }
    public string? NDCCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public List<RefillDetailDto> Refills { get; set; } = new();
}

public class RefillDetailDto
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PharmacyId { get; set; }
    public string? Notes { get; set; }
}
