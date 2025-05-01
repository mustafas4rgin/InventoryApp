using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IMapper mapper, IValidator<RegisterDTO> registerValidator, IValidator<LoginDTO> loginValidator, IAuthService authService)
        {
            _mapper = mapper;
            _registerValidator = registerValidator;
            _authService = authService;
            _loginValidator = loginValidator;
        }
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody]UpdateProfileDTO dto)
        {
            var userId = User.GetUserId();

            var existingUserResult = await _authService.Me(userId);

            if (!existingUserResult.Success)
                return NotFound(existingUserResult.Message);

            var existingUser = existingUserResult.Data;

            _mapper.Map(dto,existingUser);

            var updatingResult = await _authService.UpdateProfileAsync(existingUser);

            if (!updatingResult.Success)
                return BadRequest(updatingResult.Message);

            return Ok(updatingResult.Message);
        }
        [Authorize]
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDTO dto)
        {
            var userId = User.GetUserId();

            var result = await _authService.ResetPasswordAsync(userId,dto.OldPassword,dto.NewPassword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDTO dTO)
        {
            var validationResult = await _registerValidator.ValidateAsync(dTO);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var user = _mapper.Map<User>(dTO);

            var registerResult = await _authService.RegisterAsync(user);

            if (!registerResult.Success)
                return BadRequest(registerResult.Message);

            return Ok(registerResult.Message);
        }
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == null)
                return Unauthorized("Invalid token.");

            var result = await _authService.Me(int.Parse(userId));

            if (!result.Success)
                return BadRequest(result.Message);

            var user = result.Data;

            var dto = _mapper.Map<UserDTO>(user);

            return Ok(new
            {
                user = dto,
                supplierId = user.SupplierId
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var loginResult = await _authService.LoginAsync(dto);

            if (!loginResult.Success)
                return BadRequest(loginResult.Message);

            var token = loginResult.Data;

            return Ok(token);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO dto)
        {
            var tokenResult = await _authService.GenerateAccessTokenWithRefreshTokenAsync(dto);

            if (!tokenResult.Success)
                return BadRequest(tokenResult.Message);

            return Ok(tokenResult.Data);
        }
    }
}
