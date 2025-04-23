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
        CreateMap<Supplier,SupplierDTO>().ReverseMap();
        CreateMap<Supplier,CreateSupplierDTO>().ReverseMap();
        CreateMap<Supplier,UpdateSupplierDTO>().ReverseMap();
    }
}