using InventoryApp.Data.Context;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Data;

public class GenericRepository : IGenericRepository
{
    private readonly AppDbContext _context;
    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    public IQueryable<T> GetAll<T>() where T : EntityBase
    {
        return _context.Set<T>();
    }
    public async Task<T?> GetByIdAsync<T>(int id) where T : EntityBase
    {
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity is null)
            return null;

        return entity;
    }
    public async Task<T?> AddAsync<T>(T entity) where T : EntityBase
    {
        entity.Id = default;

        if (entity is null)
            return null;

        await _context.Set<T>().AddAsync(entity);
        entity.CreatedAt = DateTime.UtcNow;

        return entity;
    }
    public async Task<T?> UpdateAsync<T>(T entity) where T : EntityBase
    {
        if (entity.Id != default)
            return null;

        var dbEntity = await GetByIdAsync<T>(entity.Id);

        if (dbEntity is null)
            return null;

        entity.CreatedAt = dbEntity.CreatedAt;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Update(entity);

        return entity;
    }
    public void Delete<T>(T entity) where T : EntityBase
    {
        if (entity is null)
            return;
            
        _context.Set<T>().Remove(entity);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}