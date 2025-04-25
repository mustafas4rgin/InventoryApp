using AutoMapper;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Entities;

namespace InventoryApp.API.Profiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification,CreateNotificationDTO>().ReverseMap();
        CreateMap<Notification,UpdateNotificationDTO>().ReverseMap();
        CreateMap<Notification,NotificationDTO>().ReverseMap();
    }
}