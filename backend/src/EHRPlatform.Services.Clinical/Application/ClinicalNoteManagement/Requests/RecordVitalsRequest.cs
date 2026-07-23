namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Requests;

/// <summary>
/// Record vitals request.
/// </summary>
public class RecordVitalsRequest
{
    public Guid ClinicalNoteId { get; set; }
    public decimal Temperature { get; set; } // Celsius
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
}
