using AutoMapper;
using EHRPlatform.Services.Prescription.Domain;
using EHRPlatform.Services.Prescription.Application.Prescriptions.Responses;

namespace EHRPlatform.Services.Prescription.Application.Prescriptions.Mappers;

public class PrescriptionMappingProfile : Profile
{
    public PrescriptionMappingProfile()
    {
        CreateMap<Prescription, PrescriptionResponse>();
    }
}
