using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Application.Results.Raw;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Services;

public class GenericService<T> : IGenericService<T> where T : EntityBase
{
    private readonly IValidator<T> _validator;
    private readonly IGenericRepository _genericRepository;
    public GenericService(IGenericRepository genericRepository, IValidator<T> validator)
    {
        _validator = validator;
        _genericRepository = genericRepository;
    }
    public async Task<IServiceResult> RestoreAsync(int id)
    {
        try
        {
            var entity = await _genericRepository.GetByIdAsync<T>(id);

            if (entity is null || !entity.IsDeleted)
                return new ErrorResult("Entity is not exist or wasn't deleted.");

            entity.IsDeleted = false;

            await _genericRepository.UpdateAsync(entity);
            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Entity restored successfully.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResult> HardDeleteAsync(int id)
    {
        try
        {
            var entity = await _genericRepository.GetByIdAsync<T>(id);

            if (entity is null)
                return new ErrorResult($"There is no entity with ID : {id}.");

            var validationResult = await _validator.ValidateAsync(entity);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)));

            _genericRepository.Delete(entity);
            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Entity deleted successfully.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<T>>> GetAllDeletedAsync()
    {
        try
        {
            var deletedEntities = await _genericRepository.GetAll<T>()
                                .Where(e => e.IsDeleted)
                                .ToListAsync();

            if (!deletedEntities.Any())
                return new ErrorResultWithData<IEnumerable<T>>("There is no deleted entity.");

            return new SuccessResultWithData<IEnumerable<T>>("Deleted entities found.", deletedEntities);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<T>>(ex.Message);
        }
    }
    public virtual async Task<IServiceResultWithData<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            var entities = await _genericRepository.GetAll<T>()
                                .Where(e => !e.IsDeleted)
                                .ToListAsync();

            if (!entities.Any())
                return new ErrorResultWithData<IEnumerable<T>>("There is no entity.");

            return new SuccessResultWithData<IEnumerable<T>>("Entities found.", entities);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<T>>(ex.Message);
        }
    }
    public virtual async Task<IServiceResultWithData<T>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _genericRepository.GetByIdAsync<T>(id);

            if (entity is null || entity.IsDeleted)
                return new ErrorResultWithData<T>($"There is no entity with this ID: {id}");

            return new SuccessResultWithData<T>("Entity found", entity);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<T>(ex.Message);
        }
    }
    public virtual async Task<IServiceResult> AddAsync(T entity)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)));

            await _genericRepository.AddAsync(entity);

            var admins = await _genericRepository.GetAll<User>()
                            .Include(u => u.Role)
                            .Where(u => u.Role.Name == "Admin")
                            .ToListAsync();

            foreach (var admin in admins)
            {
                var title = entity?.GetType().GetProperty("Name")?.GetValue(entity)?.ToString()
           ?? entity?.GetType().GetProperty("Title")?.GetValue(entity)?.ToString()
           ?? entity.ToString();

                await _genericRepository.AddAsync(
                    new Notification
                    {
                        Title = $"{title}",
                        Message = $"{title} created",
                        Type = NotificationType.Info,
                        UserId = admin.Id,
                    }
                );
            }

            if (entity is Product product)
            {
                var supplier = await _genericRepository.GetAll<Supplier>()
                                .Include(s => s.Users)
                                .FirstOrDefaultAsync(s => s.Id == product.SupplierId);

                if (supplier is not null)
                {
                    var productName = product.Name ?? product.ToString();
                    foreach (var user in supplier.Users)
                    {
                        await _genericRepository.AddAsync(new Notification
                        {
                            Title = $"{productName}",
                            Message = $"{productName} added.",
                            Type = NotificationType.Info,
                            UserId = user.Id
                        });
                    }
                }
            }

            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Entity created.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResult> UpdateAsync(T entity)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)));

            if (entity.IsDeleted)
                return new ErrorResult("Entity no longer exists.");

            await _genericRepository.UpdateAsync(entity);

            var admins = await _genericRepository.GetAll<User>()
                                .Include(u => u.Role)
                                .Where(u => u.Role.Name == "Admin")
                                .ToListAsync();

            if (entity is Product product)
            {
                var supplier = await _genericRepository.GetAll<Supplier>()
                                .Include(s => s.Users)
                                .FirstOrDefaultAsync(s => s.Id == product.SupplierId);

                if (supplier is not null)
                {
                    var productName = product.Name ?? product.ToString();
                    foreach (var user in supplier.Users)
                    {
                        await _genericRepository.AddAsync(new Notification
                        {
                            Title = $"{productName}",
                            Message = $"{productName} uptated.",
                            Type = NotificationType.Info,
                            UserId = user.Id
                        });
                    }
                }
            }

            foreach (var admin in admins)
            {
                var title = entity?.GetType().GetProperty("Name")?.GetValue(entity)?.ToString()
                        ?? entity?.GetType().GetProperty("Title")?.GetValue(entity)?.ToString()
                        ?? entity.ToString();

                await _genericRepository.AddAsync(

                    new Notification
                    {
                        Title = $"{title}",
                        Message = $"{title} updated.",
                        Type = NotificationType.Info,
                        UserId = admin.Id
                    }
                );
            }

            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Entity uptated.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResult> DeleteAsync(int id)
    {
        try
        {
            var entity = await _genericRepository.GetByIdAsync<T>(id);

            if (entity is null)
                return new ErrorResult($"There is no entity with ID : {id}.");

            var validationResult = await _validator.ValidateAsync(entity);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)));

            if (entity.IsDeleted)
                return new ErrorResult("Entity no longer exists.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            if (entity is Product product)
            {
                var supplier = await _genericRepository.GetAll<Supplier>()
                                .Include(s => s.Users)
                                .FirstOrDefaultAsync(s => s.Id == product.SupplierId);

                if (supplier is not null)
                {
                    var productName = product.Name ?? product.ToString();
                    foreach (var user in supplier.Users)
                    {
                        await _genericRepository.AddAsync(new Notification
                        {
                            Title = $"{productName}",
                            Message = $"{productName} deleted.",
                            Type = NotificationType.Info,
                            UserId = user.Id
                        });
                    }
                }
            }

            var admins = await _genericRepository.GetAll<User>()
                            .Include(u => u.Role)
                            .Where(u => u.Role.Name == "Admin")
                            .ToListAsync();

            foreach (var admin in admins)
            {
                var title = entity?.GetType().GetProperty("Name")?.GetValue(entity)?.ToString()
                ?? entity?.GetType().GetProperty("Title")?.GetValue(entity)?.ToString()
                ?? entity.ToString();

                await _genericRepository.AddAsync(
                    new Notification
                    {
                        Title = $"{title}",
                        Message = $"{title} deleted.",
                        Type = NotificationType.Warning,
                        UserId = admin.Id
                    }
                );
            }

            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Entity deleted.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }
    }
}