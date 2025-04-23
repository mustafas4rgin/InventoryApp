using AutoMapper;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Domain.Entities;
using LibraryApp.Application.DTOs.Update;

namespace InventoryApp.API.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role,RoleDTO>().ReverseMap();
        CreateMap<Role,CreateRoleDTO>().ReverseMap();
        CreateMap<Role,UpdateRoleDTO>().ReverseMap();
    }
}