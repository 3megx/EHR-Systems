namespace EHRPlatform.Services.Prescription.Application.PrescriptionManagement.Responses;

/// <summary>
/// Refill request list DTO with pagination.
/// Single Responsibility: Represent paginated list of pending refill requests in API responses.
/// Part of Application Layer (contracts between service and clients).
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
