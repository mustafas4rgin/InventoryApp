using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface IGenericRepository
{
    IQueryable<T> GetAll<T>() where T : EntityBase;
    Task<T?> GetByIdAsync<T>(int id) where T : EntityBase;
    Task<T?> UpdateAsync<T>(T entity) where T : EntityBase;
    Task<T?> AddAsync<T>(T entity) where T : EntityBase;
    void Delete<T>(T entity) where T : EntityBase;
    Task SaveChangesAsync();
}