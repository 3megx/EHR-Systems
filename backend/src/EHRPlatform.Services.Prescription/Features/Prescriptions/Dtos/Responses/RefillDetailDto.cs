namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Dtos.Responses;

/// <summary>
/// Refill request list DTO.
/// Single Responsibility: Represent refill requests in paginated responses.
/// </summary>
public class RefillRequestListDto
{
    public List<RefillRequestDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Refill request DTO.
/// </summary>
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
