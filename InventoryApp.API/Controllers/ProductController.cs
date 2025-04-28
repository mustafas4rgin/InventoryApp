using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using FluentValidation;
using InventoryApp.API.Controllers;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : GenericController<Product,CreateProductDTO,UpdateProductDTO,ProductDTO>
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductController(
            IProductService service,
            IValidator<CreateProductDTO> createValidator,
            IValidator<UpdateProductDTO> updateValidator,
            IMapper mapper
        ) : base(service,createValidator,updateValidator,mapper) 
        {
            _mapper = mapper;
            _productService = service;
        }
        public async override Task<IActionResult> GetAll(string? include,string? search)
        {
            var result = await _productService.GetProductsWithIncludeAsync(include,search);

            if (!result.Success)
                return NotFound(result.Message);

            var products = result.Data;

            var dto = _mapper.Map<List<ProductDTO>>(products);

            return Ok(dto);
        }
        public async override Task<IActionResult> GetById(int id, string? include)
        {
            var result = await _productService.GetProductByIdWithIncludeAsync(include,id);

            if (!result.Success)
                return NotFound(result.Message);


            var product = result.Data;

            var dto = _mapper.Map<ProductDTO>(product);

            return Ok(dto);
        }
    }
}
