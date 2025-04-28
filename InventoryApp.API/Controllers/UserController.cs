using AutoMapper;
using FluentValidation;
using InventoryApp.API.Controllers;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : GenericController<User, CreateUserDTO, UpdateUserDTO, UserDTO>
    {
        private readonly IValidator<UpdateUserDTO> _updateValidator;
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserDTO> _createValidator;
        private readonly IMapper _mapper;
        public UserController(IUserService userService,
        IValidator<CreateUserDTO> createValidator,
        IValidator<UpdateUserDTO> updateValidator,
        IMapper mapper
        ) : base(userService, createValidator, updateValidator, mapper)
        {
            _updateValidator = updateValidator;
            _createValidator = createValidator;
            _userService = userService;
            _mapper = mapper;
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Approve/{userId}")]
        public async Task<IActionResult> ApproveUser(int userId)
        {
            var result = await _userService.ApproveUserAsync(userId);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
        public override async Task<IActionResult> Add(CreateUserDTO dto)
        {
            var dtoValidationResult = await _createValidator.ValidateAsync(dto);

            if (!dtoValidationResult.IsValid)
                return BadRequest(dtoValidationResult.Errors);

            var user = _mapper.Map<User>(dto);

            var addingResult = await _userService.AddUserAsync(user);

            if (!addingResult.Success)
                return BadRequest(addingResult.Message);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, addingResult);
        }
        [Authorize(Roles ="Admin")]
        public override async Task<IActionResult> GetAll(string? include,string? search)
        {
            var result = await _userService.GetAllUsersWithIncludeAsync(include,search);

            if (!result.Success)
                return NotFound(result.Message);

            var users = result.Data;

            var dto = _mapper.Map<List<UserDTO>>(users);

            return Ok(dto);
        }
        public override async Task<IActionResult> GetById(int id, string? include)
        {
            var result = await _userService.GetUserByIdWithIncludeAsync(include, id);

            if (!result.Success)
                return NotFound(result.Message);

            var user = result.Data;

            var dto = _mapper.Map<UserDTO>(user);

            return Ok(dto);
        }
        public override async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserDTO dto)
        {
            var dtoValidationResult = await _updateValidator.ValidateAsync(dto);

            if (!dtoValidationResult.IsValid)
                return BadRequest(dtoValidationResult.Errors);

            var existingUserResult = await _userService.GetUserByIdWithIncludeAsync(null, id); // << BURASI DÜZELDİ

            if (!existingUserResult.Success)
                return NotFound(existingUserResult.Message);

            var existingUser = existingUserResult.Data;

            _mapper.Map(dto, existingUser);

            var updatingResult = await _userService.UpdateUserAsync(existingUser); // dikkat! Update değil UpdateUserAsync

            if (!updatingResult.Success)
                return BadRequest(updatingResult.Message);

            return Ok(updatingResult.Message);
        }
        [HttpPut("{userId}/Role/{roleId}")]
        public async Task<IActionResult> UpdateRole([FromRoute]int userId,[FromRoute]int roleId)
        {
            var result = await _userService.UpdateRoleAsync(userId,roleId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
