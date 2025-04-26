using AutoMapper;
using FluentValidation;
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
    public class GenericController<T,TCreateDto,TUpdateDto,TListDto> : ControllerBase
    where T : EntityBase
    where TCreateDto : class
    where TUpdateDto : class
    where TListDto : class
    {
        private readonly IGenericService<T> _genericService;
        private readonly IValidator<TCreateDto> _createValidator;
        private readonly IValidator<TUpdateDto> _updateValidator;
        private readonly IMapper _mapper;
        
        public GenericController(
            IGenericService<T> genericService,
            IValidator<TCreateDto> createValidator,
            IValidator<TUpdateDto> updateValidator,
            IMapper mapper
        )
        {
            _genericService = genericService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public virtual async Task<IActionResult> GetAll(string? include)
        {
            var result = await _genericService.GetAllAsync();

            if (!result.Success)
                return NotFound(result.Message);

            var entities = result.Data;

            var dto = _mapper.Map<List<TListDto>>(entities);

            return Ok(dto);
        }
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id,string? include)
        {
            var result = await _genericService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result.Message);

            var entity = result.Data;

            var dto = _mapper.Map<TListDto>(entity);

            return Ok(dto);
        }
        [HttpPost("Add")]
        public virtual async Task<IActionResult> Add(TCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var entity = _mapper.Map<T>(dto);

            var result = await _genericService.AddAsync(entity);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
        [HttpPut("Update/{id}")]
        public virtual async Task<IActionResult> Update([FromRoute]int id,[FromBody]TUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var existingEntityResult = await _genericService.GetByIdAsync(id);

            if (!existingEntityResult.Success)
                return NotFound(existingEntityResult.Message);

            var existingEntity = existingEntityResult.Data;

            _mapper.Map(dto,existingEntity);

            var updatingResult = await _genericService.UpdateAsync(existingEntity);

            if (!updatingResult.Success)
                return BadRequest(updatingResult.Message);

            return Ok(updatingResult.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var existingEntityResult = await _genericService.GetByIdAsync(id);

            if (!existingEntityResult.Success)
                return NotFound(existingEntityResult.Message);

            var entity = existingEntityResult.Data;

            var deletingResult = await _genericService.DeleteAsync(entity);

            if (!deletingResult.Success)
                return BadRequest(deletingResult.Message);

            return Ok(deletingResult.Message);
        }
    }
}
