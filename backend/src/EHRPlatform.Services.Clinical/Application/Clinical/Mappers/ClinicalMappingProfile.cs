using AutoMapper;
using EHRPlatform.Services.Clinical.Domain.Entities;
using EHRPlatform.Services.Clinical.Application.Clinical.Responses;

namespace EHRPlatform.Services.Clinical.Application.Clinical.Mappers;

public class ClinicalMappingProfile : Profile
{
    public ClinicalMappingProfile()
    {
        CreateMap<ClinicalNote, ClinicalNoteResponse>();
    }
}
