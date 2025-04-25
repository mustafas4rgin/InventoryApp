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
    }
}
