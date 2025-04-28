using AutoMapper;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Entities;

namespace InventoryApp.API.Profiles;

public class SupplierProfile : Profile
{
    public SupplierProfile()
    {
        CreateMap<Supplier, SupplierDTO>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src =>
                src.Users
                   .Where(u => !u.IsDeleted)
                   .Select(u => new UserDTO
                   {
                       Id = u.Id,
                       FirstName = u.FirstName,
                       LastName = u.LastName
                   })
                   .ToList()
            ))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src =>
                src.Products
                   .Where(p => !p.IsDeleted)
                   .Select(p => p.Name)
                   .ToList()
            ));
        CreateMap<Supplier,CreateSupplierDTO>().ReverseMap();
        CreateMap<Supplier,UpdateSupplierDTO>().ReverseMap();
    }
}