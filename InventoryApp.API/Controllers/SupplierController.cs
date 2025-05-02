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
    [Authorize(Roles ="Supplier")]
    [ApiController]
    public class SupplierController : GenericController<Supplier,CreateSupplierDTO,UpdateSupplierDTO,SupplierDTO>
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;
        public SupplierController(
            ISupplierService service,
            IValidator<CreateSupplierDTO> createValidator,
            IValidator<UpdateSupplierDTO> updateValidator,
            IMapper mapper
        ) : base (service,createValidator,updateValidator,mapper) 
        {
            _mapper = mapper;
            _supplierService = service;
        }
        public async override Task<IActionResult> GetAll([FromQuery]string? include, [FromQuery]string? search)
        {
            var result = await _supplierService.GetAllSuppliersWithIncludeAsync(include,search);

            if (!result.Success)
                return NotFound(result.Message);

            var suppliers = result.Data;

            var dto = _mapper.Map<List<SupplierDTO>>(suppliers);

            return Ok(dto);
        }
        public async override Task<IActionResult> GetById(int id, string? include)
        {
            var result = await _supplierService.GetSupplierByIdWithIncludeAsync(include,id);

            if (!result.Success)
                return NotFound(result.Message);

            var supplier = result.Data;

            var dto = _mapper.Map<SupplierDTO>(supplier);

            return Ok(dto);
        }
    }
}
