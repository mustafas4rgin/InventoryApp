using FluentValidation;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Application.Results.Raw;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Services;

public class UserService : GenericService<User>, IUserService
{
    private readonly IValidator<User> _validator;
    private readonly IGenericRepository _genericRepository;
    public UserService(
    IValidator<User> validator,
    IGenericRepository genericRepository
    ) : base(genericRepository, validator)
    {
        _genericRepository = genericRepository;
        _validator = validator;
    }
    public async Task<IServiceResult> UpdateUserAsync(User user)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(user);

        if (!validationResult.IsValid)
            return new ErrorResult(
                validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .Aggregate((a, b) => $"{a} | {b}")
            );

        var existingUser = await _genericRepository.GetAll<User>()
                                .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (existingUser is not null)
            return new ErrorResult("There is a user with same email.");

        var role = await _genericRepository.GetByIdAsync<Role>(user.RoleId);

        if (role is null)
            return new ErrorResult($"There is no role with ID : {user.RoleId}");

        var supplier = await _genericRepository.GetByIdAsync<Supplier>(user.SupplierId);

        if (supplier is null)
            return new ErrorResult($"There is no supplier with Id : {user.SupplierId}");

        await _genericRepository.UpdateAsync(user);
        await _genericRepository.SaveChangesAsync();

        return new SuccessResult("User updated successfully.");
    }
    catch (Exception ex)
    {
        return new ErrorResult(ex.Message);
    }
        }
        
    public async Task<IServiceResultWithData<User>> GetUserByIdWithIncludeAsync(string? include, int id)
    {
        try
        {
            var query = _genericRepository.GetAll<User>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',',StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "supplier")
                        query = query.Include(u => u.Supplier);
                    else if (inc == "role")
                        query = query.Include(u => u.Role);
                    else if (inc == "all")
                        query = query.Include(u => u.Supplier)
                                    .Include(u => u.Role);
                }
            }

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return new ErrorResultWithData<User>($"There is no user with ID : {id}");

            return new SuccessResultWithData<User>("User found.",user);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<User>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<User>>> GetAllUsersWithIncludeAsync(string? include)
    {
        try
        {
            var query = _genericRepository.GetAll<User>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "supplier")
                        query = query.Include(u => u.Supplier);
                    else if (inc == "role")
                        query = query.Include(u => u.Role);
                    else if (inc == "all")
                        query = query.Include(u => u.Role)
                                    .Include(u => u.Supplier);
                }
            }

            var users = await query.ToListAsync();

            if (!users.Any())
                return new ErrorResultWithData<IEnumerable<User>>("There is no user.");

            return new SuccessResultWithData<IEnumerable<User>>("Users found.", users);

        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<User>>(ex.Message);
        }
    }
    public async Task<IServiceResult> AddUserAsync(User user)
    {
        var validationResult = await _validator.ValidateAsync(user);

        if (!validationResult.IsValid)
            return new ErrorResult(
                validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .Aggregate((a, b) => $"{a} | {b}")
            );

        var existingRole = await _genericRepository.GetByIdAsync<Role>(user.RoleId);

        if (existingRole is null)
            return new ErrorResult($"There is no role with ID : {user.RoleId}.");

        var existingSupplier = await _genericRepository.GetByIdAsync<Supplier>(user.SupplierId);

        if (existingSupplier is null)
            return new ErrorResult($"There is no supplier with Id : {user.SupplierId}.");

        var existingUser = await _genericRepository.GetAll<User>()
                            .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (existingUser is not null)
            return new ErrorResult("There is a user with this email.");

        await _genericRepository.AddAsync(user);
        await _genericRepository.SaveChangesAsync();

        return new SuccessResult("User created successfully.");
    }
    public override Task<IServiceResult> AddAsync(User user)
    {
        throw new NotSupportedException("Use AddUserAsync instead for User entities.");
    }
    public override Task<IServiceResultWithData<IEnumerable<User>>> GetAllAsync()
    {
        throw new NotSupportedException("Use GetAllUsersWithUncludesAsync instead for User entities.");
    }
    public override Task<IServiceResultWithData<User>> GetByIdAsync(int id)
    {
        throw new NotSupportedException("Use GetUserByIdWithIncludeAsync instead of GetByIdAsync.");
    }
}