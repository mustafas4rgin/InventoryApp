using System.Security.Claims;
using FluentValidation;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IAuthService _authService;

        public AuthController(IValidator<LoginDTO> loginValidator, IAuthService authService)
        {
            _authService = authService;
            _loginValidator = loginValidator;
        }
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var firstName = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (userId == null)
                return Unauthorized("Invalid token.");

            return Ok(new
            {
                Id = userId,
                FirstName = firstName,
                Role = role
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
