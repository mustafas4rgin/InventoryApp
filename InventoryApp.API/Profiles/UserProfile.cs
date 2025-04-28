using AutoMapper;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Application.Helpers;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                HashingHelper.CreatePasswordHash(src.Password, out var hash, out var salt);
                dest.PasswordHash = hash;
                dest.PasswordSalt = salt;
                dest.RoleId = 3;
            });

        CreateMap<UpdateUserDTO, User>().ReverseMap();
        CreateMap<RegisterDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
            .AfterMap((src,dest) => 
            {
                HashingHelper.CreatePasswordHash(src.Password, out var hash, out var salt);
                dest.PasswordHash = hash;
                dest.PasswordSalt = salt;
            });

        CreateMap<User, UserDTO>()
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd.MM.yyyy HH:mm")))
    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.ToString("dd.MM.yyyy HH:mm") : null))
    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
    .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));

    }
}
