using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using InventoryApp.Application.DTOs.Update;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : GenericController<Role,CreateRoleDTO,UpdateRoleDTO,RoleDTO>
    {
        public RoleController(IGenericService<Role> service,
        IValidator<CreateRoleDTO> createValidator,
        IValidator<UpdateRoleDTO> updateValidator,
        IMapper mapper
        ) : base(service,createValidator,updateValidator,mapper) {}
    }
}
