using AutoMapper;
using EHRPlatform.Services.Identity.Domain.Entities;
using EHRPlatform.Services.Identity.Application.Auth.Responses;

namespace EHRPlatform.Services.Identity.Application.Auth.Mappers;

/// <summary>
/// AutoMapper profile for Auth entities.
/// </summary>
public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, LoginResponse>();
    }
}
