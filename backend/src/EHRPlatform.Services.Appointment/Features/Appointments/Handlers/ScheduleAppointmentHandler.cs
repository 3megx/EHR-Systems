using MediatR;
using EHRPlatform.Services.Appointment.Features.Appointments.Commands;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Handlers;

public class ScheduleAppointmentHandler : IRequestHandler<ScheduleAppointmentCommand, object>
{
    public Task<object> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class ScheduleAppointmentCommand : IRequest<object>
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
}
