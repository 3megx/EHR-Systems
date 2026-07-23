namespace EHRPlatform.Services.Patient.Features.Patients.Dtos.Responses;

/// <summary>
/// Patient list DTO with pagination.
/// Single Responsibility: Represent paginated patient collection.
/// </summary>
public class PatientListDto
{
    public List<PatientResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
