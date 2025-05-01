using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface IGenericService<T> where T : EntityBase
{
    Task<IServiceResultWithData<IEnumerable<T>>> GetAllAsync();
    Task<IServiceResultWithData<T>> GetByIdAsync(int id);
    Task<IServiceResult> AddAsync(T entity);
    Task<IServiceResult> UpdateAsync(T entity);
    Task<IServiceResult> DeleteAsync(int id);
    Task<IServiceResultWithData<IEnumerable<T>>> GetAllDeletedAsync();
    Task<IServiceResult> HardDeleteAsync(int id);
    Task<IServiceResult> RestoreAsync(int id);
}