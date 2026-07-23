namespace EHRPlatform.Services.Prescription.Application.PrescriptionManagement.Responses;

/// <summary>
/// Prescription list DTO with pagination.
/// Single Responsibility: Represent paginated list of prescriptions in API responses.
/// Part of Application Layer (contracts between service and clients).
/// </summary>
public class PrescriptionListDto
{
    public List<PrescriptionResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
