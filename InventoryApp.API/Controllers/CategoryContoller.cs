using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : GenericController<Category,CreateCategoryDTO,UpdateCategoryDTO,CategoryDTO>
    {
        public CategoryController(
            IGenericService<Category> service,
            IValidator<CreateCategoryDTO> createValidator,
            IValidator<UpdateCategoryDTO> updateValidator,
            IMapper mapper
        ) : base(service,createValidator,updateValidator,mapper) { }
    }
}
