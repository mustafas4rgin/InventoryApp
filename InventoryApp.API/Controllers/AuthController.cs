using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
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
