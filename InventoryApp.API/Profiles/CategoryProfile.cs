using AutoMapper;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Entities;

namespace InventoryApp.API.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category,CreateCategoryDTO>().ReverseMap();
        CreateMap<Category,UpdateCategoryDTO>().ReverseMap();
        CreateMap<Category,CategoryDTO>().ReverseMap();
    }
}