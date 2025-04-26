using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Application.Results.Raw;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config)
    {
        _config = config;
    }
    public IServiceResultWithData<JwtSecurityToken> GenerateJwtAccessToken(User user)
    {
        try
        {
            var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds);


            return new SuccessResultWithData<JwtSecurityToken>("Jwt access token created.", token);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<JwtSecurityToken>(ex.Message);
        }
    }
    public IServiceResultWithData<RefreshToken> GenerateRefreshToken(User user)
    {
        try
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshToken = new RefreshToken
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            return new SuccessResultWithData<RefreshToken>("Refresh token created.", refreshToken);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<RefreshToken>(ex.Message);
        }


    }
}