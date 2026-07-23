namespace EHRPlatform.Services.Prescription.Application.PrescriptionManagement.Requests;

/// <summary>
/// Request refill request DTO.
/// Single Responsibility: Represent refill request from API client.
/// Part of Application Layer (contracts between service and clients).
/// </summary>
public class RequestRefillRequest
{
    public Guid PrescriptionId { get; set; }
    public string? PharmacyId { get; set; }
}
