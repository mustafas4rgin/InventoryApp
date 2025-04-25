using AutoMapper;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Entities;

namespace InventoryApp.API.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product,CreateProductDTO>().ReverseMap();
        CreateMap<Product,UpdateProductDTO>().ReverseMap();
        CreateMap<Product,ProductDTO>().ReverseMap();
    }
}