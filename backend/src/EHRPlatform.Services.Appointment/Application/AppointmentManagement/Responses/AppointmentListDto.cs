namespace EHRPlatform.Services.Appointment.Application.AppointmentManagement.Responses;

/// <summary>
/// Appointment list response.
/// </summary>
public class AppointmentListDto
{
    public List<AppointmentResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

