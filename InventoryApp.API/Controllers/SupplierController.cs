using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : GenericController<Supplier,CreateSupplierDTO,UpdateSupplierDTO,SupplierDTO>
    {
        public SupplierController(
            IGenericService<Supplier> service,
            IValidator<CreateSupplierDTO> createValidator,
            IValidator<UpdateSupplierDTO> updateValidator,
            IMapper mapper
        ) : base (service,createValidator,updateValidator,mapper) {}
    }
}
