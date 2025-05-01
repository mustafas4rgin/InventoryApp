using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
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
    private readonly IValidator<User> _validator;
    public AuthService(IValidator<User> validator, IGenericRepository genericRepository, ITokenService tokenService)
    {
        _validator = validator;
        _genericRepository = genericRepository;
        _tokenService = tokenService;
    }
    public async Task<IServiceResult> UpdateProfileAsync(User user)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(user);

            if (!validationResult.IsValid)
                return new ErrorResult(
                    validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .Aggregate((a, b) => $"{a} | {b}")
                );

            await _genericRepository.AddAsync(
                new Notification
                {
                    Title = "Profile",
                    Message = "Profile updated successfully.",
                    Type = NotificationType.Success,
                    UserId = user.Id
                }
            );

            await _genericRepository.UpdateAsync(user);
            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("User updated successfully.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResult> ResetPasswordAsync(int userId, string oldPassword, string newPassword)
    {
        try
        {
            var user = await _genericRepository.GetByIdAsync<User>(userId);

            if (user is null)
                return new ErrorResult("User not found.");

            if (!HashingHelper.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
                return new ErrorResult("Wrong old password.");

            HashingHelper.CreatePasswordHash(newPassword, out var hash, out var salt);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            var admins = await _genericRepository.GetAll<User>().Include(u => u.Role).Where(u => u.Role.Name == "Admin").ToListAsync();

            foreach (var admin in admins)
            {
                await _genericRepository.AddAsync(
                new Notification
                {
                    Title = $"About user : {user.FirstName}",
                    Message = $"{user.FirstName} changed password.",
                    UserId = admin.Id,
                    Type = NotificationType.Info,
                }
            );
            }

            await _genericRepository.AddAsync(
                new Notification
                {
                    Title = "Password Change",
                    Message = "Your password changed successfully.",
                    UserId = user.Id,
                    Type = NotificationType.Success
                }
            );

            await _genericRepository.UpdateAsync(user);
            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Password changed successfully.");

        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }


    }
    public async Task<IServiceResultWithData<User>> Me(int userId)
    {
        var user = await _genericRepository.GetAll<User>().Include(u => u.Role).Include(u => u.Supplier).FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ErrorResultWithData<User>($"Invalid token.");

        return new SuccessResultWithData<User>("Me: ", user);
    }
    public async Task<IServiceResult> RegisterAsync(User user)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(user);

            if (!validationResult.IsValid)
                return new ErrorResult(
                    validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .Aggregate((a, b) => $"{a} | {b}")
                );

            var existingUser = await _genericRepository.GetAll<User>().FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser is not null)
                return new ErrorResult("There is a user with this email.");

            var existingRole = await _genericRepository.GetAll<Role>().FirstOrDefaultAsync(r => r.Id == user.RoleId);

            if (existingRole is null || existingRole.IsDeleted)
                return new ErrorResult($"There is no role with ID : {user.RoleId}");

            var existingSupplier = await _genericRepository.GetAll<Supplier>().FirstOrDefaultAsync(s => s.Id == user.SupplierId);

            if (existingSupplier is null || existingSupplier.IsDeleted)
                return new ErrorResult($"There is no supplier with ID : {user.Supplier}");

            await _genericRepository.AddAsync(user);

            var admins = _genericRepository.GetAll<User>()
                            .Include(u => u.Role)
                            .Where(u => u.Role.Name == "Admin");

            foreach (var admin in admins)
            {
                await _genericRepository.AddAsync(new Notification
                {
                    Title = "New User",
                    Message = "New user registered and waiting for approval.",
                    UserId = admin.Id,
                    Type = NotificationType.Info,
                });
            }

            await _genericRepository.AddAsync(new Notification
            {
                UserId = user.Id,
                Title = "Register",
                Message = "Thank you for registration.",
                Type = NotificationType.Info,

            });

            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("User registered.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
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