using MediatR;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Queries;

public class GetAppointmentsQuery : IRequest<IEnumerable<object>>
{
    public Guid? PatientId { get; set; }
    public Guid? ProviderId { get; set; }
}
