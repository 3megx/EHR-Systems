namespace EHRPlatform.Services.Clinical.Application.ClinicalNotes.Responses;

/// <summary>
/// Vital signs nested DTO.
/// </summary>
public class VitalSignsDto
{
    public Guid Id { get; set; }
    public decimal Temperature { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
    public DateTime RecordedAt { get; set; }
}
