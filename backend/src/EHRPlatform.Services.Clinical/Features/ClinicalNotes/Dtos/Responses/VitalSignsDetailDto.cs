namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Dtos.Responses;

/// <summary>
/// Vital signs detail DTO.
/// Single Responsibility: Represent vital signs timeline and statistics.
/// </summary>
public class VitalSignsDetailDto
{
    public Guid PatientId { get; set; }
    public List<VitalSignsRecordDto> Records { get; set; } = new();
    public VitalSignsStatisticsDto Statistics { get; set; } = new();
}

public class VitalSignsRecordDto
{
    public Guid Id { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Temperature { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
}

public class VitalSignsStatisticsDto
{
    public decimal AverageTemperature { get; set; }
    public int AverageSystolicBP { get; set; }
    public int AverageDiastolicBP { get; set; }
    public int AverageHeartRate { get; set; }
}
