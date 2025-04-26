using InventoryApp.Application.DTOs;
using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Interfaces;

public interface IAuthService
{
    Task<IServiceResultWithData<TokenResponseDTO>> LoginAsync(LoginDTO dto); 
    Task<IServiceResultWithData<TokenResponseDTO>> GenerateAccessTokenWithRefreshTokenAsync(RefreshTokenRequestDTO dto);

}