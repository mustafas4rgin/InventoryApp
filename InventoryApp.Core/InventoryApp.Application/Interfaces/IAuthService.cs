using InventoryApp.Application.DTOs;
using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Interfaces;

public interface IAuthService
{
    Task<IServiceResult> UpdateProfileAsync(User user);
    Task<IServiceResult> ResetPasswordAsync(int userId, string oldPassword, string newPassword);
    Task<IServiceResultWithData<User>> Me(int userId);
    Task<IServiceResult> RegisterAsync(User user);
    Task<IServiceResultWithData<TokenResponseDTO>> LoginAsync(LoginDTO dto); 
    Task<IServiceResultWithData<TokenResponseDTO>> GenerateAccessTokenWithRefreshTokenAsync(RefreshTokenRequestDTO dto);

}