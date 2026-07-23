using AutoMapper;
using EHRPlatform.Services.Patient.Domain.Entities;
using EHRPlatform.Services.Patient.Application.Patients.Responses;

namespace EHRPlatform.Services.Patient.Application.Patients.Mappers;

public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Entities.Patient, PatientResponse>();
    }
}
