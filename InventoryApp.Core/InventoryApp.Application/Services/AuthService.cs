using System.IdentityModel.Tokens.Jwt;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Helpers;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Application.Results.Raw;
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
    public async Task<IServiceResult> LogOutAsync(string accessTokenString)
    {
        var accessToken = await _genericRepository.GetAll<AccessToken>()
                                .FirstOrDefaultAsync(at => at.Token == accessTokenString);

        if (accessToken is null)
            return new ErrorResult("Invalid token.");

        var refreshToken = await _genericRepository.GetAll<RefreshToken>()
                                .FirstOrDefaultAsync(rt => rt.Token == accessToken.RefreshToken);

        if (refreshToken is null)
            return new ErrorResult("Refresh token not found.");

        accessToken.IsRevoked = true;
        refreshToken.IsRevoked = true;

        await _genericRepository.UpdateAsync(accessToken);
        await _genericRepository.UpdateAsync(refreshToken);
        await _genericRepository.SaveChangesAsync();

        return new SuccessResult("Logged out successfully.");
    }
    public async Task<IServiceResultWithData<TokenResponseDTO>> GenerateAccessTokenWithRefreshTokenAsync(RefreshTokenRequestDTO dto)
    {
        var refreshToken = await _genericRepository.GetAll<RefreshToken>()
                            .FirstOrDefaultAsync(rt => rt.Token == dto.Token);
    
        if (refreshToken is null)
            return new ErrorResultWithData<TokenResponseDTO>("Invalid token.");

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            _genericRepository.Delete(refreshToken);
            await _genericRepository.SaveChangesAsync();

            return new ErrorResultWithData<TokenResponseDTO>("Token expired.");
        }
            
        if (refreshToken.IsUsed)
            return new ErrorResultWithData<TokenResponseDTO>("Refresh token has already been used.");


        var user = await _genericRepository.GetByIdAsync<User>(refreshToken.UserId);

        if (user is null)
            return new ErrorResultWithData<TokenResponseDTO>("User no longer exist.");

        var accessTokenResult = _tokenService.GenerateJwtAccessToken(user);

        if (!accessTokenResult.Success)
            return new ErrorResultWithData<TokenResponseDTO>("Access token error.");

        var accessToken = new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(accessTokenResult.Data),
            ExpiresAt = accessTokenResult.Data.ValidTo,
            RefreshToken = refreshToken.Token
        };

        refreshToken.IsUsed = true;

        await _genericRepository.AddAsync(accessToken);
        await _genericRepository.SaveChangesAsync();

        var result = new TokenResponseDTO
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };

        return new SuccessResultWithData<TokenResponseDTO>("Access token updated.", result);
    }
    public async Task<IServiceResultWithData<TokenResponseDTO>> LoginAsync(LoginDTO dto)
    {
        var user = await _genericRepository.GetAll<User>()
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !HashingHelper.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt) || user.IsDeleted)
            return new ErrorResultWithData<TokenResponseDTO>("Invalid email or password.");

        var accessTokenResult = _tokenService.GenerateJwtAccessToken(user);

        if (!accessTokenResult.Success)
            return new ErrorResultWithData<TokenResponseDTO>(accessTokenResult.Message);

        var refreshTokenResult = _tokenService.GenerateRefreshToken(user);

        if (!refreshTokenResult.Success)
            return new ErrorResultWithData<TokenResponseDTO>(refreshTokenResult.Message);

        var refreshToken = refreshTokenResult.Data;

        var token = accessTokenResult.Data;
        var expiresAt = token.ValidTo;

        var accessToken = new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            UserId = user.Id,
            RefreshToken = refreshToken.Token
        };

        var response = new TokenResponseDTO
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };

        await _genericRepository.AddAsync(refreshToken);
        await _genericRepository.AddAsync(accessToken);
        await _genericRepository.SaveChangesAsync();

        return new SuccessResultWithData<TokenResponseDTO>("Login successful.", response);
    }
}