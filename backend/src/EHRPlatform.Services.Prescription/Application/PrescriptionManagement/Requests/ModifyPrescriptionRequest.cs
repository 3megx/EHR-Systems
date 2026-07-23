namespace EHRPlatform.Services.Prescription.Application.PrescriptionManagement.Requests;

/// <summary>
/// Modify prescription request DTO.
/// Single Responsibility: Represent prescription status modification requests from API client.
/// Part of Application Layer (contracts between service and clients).
/// </summary>
public class SuspendPrescriptionRequest
{
    public Guid PrescriptionId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ResumePrescriptionRequest
{
    public Guid PrescriptionId { get; set; }
}

public class DiscontinuePrescriptionRequest
{
    public Guid PrescriptionId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
