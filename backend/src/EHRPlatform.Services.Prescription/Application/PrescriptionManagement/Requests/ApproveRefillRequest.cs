namespace EHRPlatform.Services.Prescription.Application.PrescriptionManagement.Requests;

/// <summary>
/// Approve refill request DTO.
/// Single Responsibility: Represent refill approval request from API client.
/// Part of Application Layer (contracts between service and clients).
/// </summary>
public class ApproveRefillRequest
{
    public Guid PrescriptionId { get; set; }
    public Guid RefillId { get; set; }
}
