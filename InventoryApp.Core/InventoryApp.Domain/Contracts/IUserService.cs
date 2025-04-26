using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface IUserService : IGenericService<User>
{
    Task<IServiceResultWithData<User>> GetUserByIdWithIncludeAsync(string? include, int id);
    Task<IServiceResultWithData<IEnumerable<User>>> GetAllUsersWithIncludeAsync(string? include);
    Task<IServiceResult> AddUserAsync(User user);
    Task<IServiceResult> UpdateUserAsync(User user);
    Task<IServiceResult> UpdateRoleAsync(int userId,int roleId);

}