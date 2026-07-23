namespace EHRPlatform.Services.Prescription.Application.Prescriptions.Responses;

public class PrescriptionResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string? MedicationName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public int Quantity { get; set; }
    public string? Status { get; set; }
}
