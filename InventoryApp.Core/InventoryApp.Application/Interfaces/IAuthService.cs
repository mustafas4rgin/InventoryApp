using InventoryApp.Application.DTOs;
using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Interfaces;

public interface IAuthService
{
    Task<IServiceResultWithData<AccessToken>> LoginAsync(LoginDTO dto); 
}