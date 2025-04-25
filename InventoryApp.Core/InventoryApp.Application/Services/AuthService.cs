using System.IdentityModel.Tokens.Jwt;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Helpers;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository _genericRepository;
    private readonly ITokenService _tokenService;
    public AuthService(IGenericRepository genericRepository, ITokenService tokenService)
    {
        _genericRepository = genericRepository;
        _tokenService = tokenService;
    }
    public async Task<IServiceResultWithData<AccessToken>> LoginAsync(LoginDTO dto)
    {
        var user = await _genericRepository.GetAll<User>()
                        .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !HashingHelper.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
            return new ErrorResultWithData<AccessToken>("Invalid email or password.");

        var accessTokenResult = _tokenService.GenerateJwtAccessToken(user);

        if (!accessTokenResult.Success)
            return new ErrorResultWithData<AccessToken>(accessTokenResult.Message);

        var token = accessTokenResult.Data;
        var expiresAt = token.ValidTo;

        var accessToken = new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            UserId = user.Id,
        };

        await _genericRepository.AddAsync(accessToken);
        await _genericRepository.SaveChangesAsync();

        return new SuccessResultWithData<AccessToken>("Access token generated.",accessToken);
    }
}