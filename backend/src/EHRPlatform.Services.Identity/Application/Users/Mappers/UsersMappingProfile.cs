using AutoMapper;
using EHRPlatform.Services.Identity.Domain.Entities;
using EHRPlatform.Services.Identity.Application.Users.Responses;

namespace EHRPlatform.Services.Identity.Application.Users.Mappers;

/// <summary>
/// AutoMapper profile for User entities.
/// </summary>
public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}
