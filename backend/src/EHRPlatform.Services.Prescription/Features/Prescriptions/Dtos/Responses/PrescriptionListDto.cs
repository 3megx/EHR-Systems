namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Dtos.Responses;

/// <summary>
/// Prescription list DTO.
/// Single Responsibility: Represent prescription list in paginated responses.
/// </summary>
public class PrescriptionListDto
{
    public List<PrescriptionResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
