using AutoMapper;
using EHRPlatform.Services.Notification.Domain.Entities;
using EHRPlatform.Services.Notification.Application.Notifications.Responses;

namespace EHRPlatform.Services.Notification.Application.Notifications.Mappers;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationResponse>();
    }
}
