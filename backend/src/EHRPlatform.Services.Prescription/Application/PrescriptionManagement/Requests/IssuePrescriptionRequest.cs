namespace EHRPlatform.Services.Prescription.Application.PrescriptionManagement.Requests;

/// <summary>
/// Issue prescription request DTO.
/// Single Responsibility: Represent prescription creation request from API client.
/// Part of Application Layer (contracts between service and clients).
/// </summary>
public class IssuePrescriptionRequest
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public string FormType { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int RefillsAllowed { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Indications { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool IsControlledSubstance { get; set; }
    public string? NDCCode { get; set; }
}
