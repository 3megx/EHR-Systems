using AutoMapper;
using EHRPlatform.Services.Appointment.Domain.Entities;
using EHRPlatform.Services.Appointment.Application.Appointments.Responses;

namespace EHRPlatform.Services.Appointment.Application.Appointments.Mappers;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<Appointment, AppointmentResponse>();
    }
}
