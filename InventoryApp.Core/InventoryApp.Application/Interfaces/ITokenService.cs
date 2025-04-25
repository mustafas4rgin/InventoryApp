using System.IdentityModel.Tokens.Jwt;
using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Interfaces;

public interface ITokenService
{
    IServiceResultWithData<JwtSecurityToken> GenerateJwtAccessToken(User user);
    IServiceResultWithData<string> GenerateRefreshToken();
}