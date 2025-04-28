using FluentValidation;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Services;

public class SupplierService : GenericService<Supplier>, ISupplierService
{
    private readonly IGenericRepository _genericRepository;
    public SupplierService(
        IGenericRepository genericRepository,
        IValidator<Supplier> validator
    ) : base(genericRepository, validator)
    {
        _genericRepository = genericRepository;
    }
    public async Task<IServiceResultWithData<IEnumerable<Supplier>>> GetAllSuppliersWithIncludeAsync(string? include, string? search)
    {
        try
        {
            var query = _genericRepository.GetAll<Supplier>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "users")
                        query = query.Include(s => s.Users);
                    else if (inc == "products")
                        query = query.Include(S => S.Products);
                    else if (inc == "all")
                        query = query.Include(u => u.Products)
                                    .Include(u => u.Users);
                }
            }
            
            var suppliers = await query.Where(s => !s.IsDeleted)
                                .Where(s => string.IsNullOrEmpty(search) || s.Name.ToLower().Contains(search.ToLower()))
                                .ToListAsync();
            


            if (!suppliers.Any())
                return new ErrorResultWithData<IEnumerable<Supplier>>("There is no supplier.");

            return new SuccessResultWithData<IEnumerable<Supplier>>("Suppliers found.", suppliers);

        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<Supplier>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<Supplier>> GetSupplierByIdWithIncludeAsync(string? include, int id)
    {
        try
        {
            var query = _genericRepository.GetAll<Supplier>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "users")
                        query = query.Include(s => s.Users);
                    else if (inc == "products")
                        query = query.Include(s => s.Products);
                    else if (inc == "all")
                        query = query.Include(s => s.Users)
                            .Include(s => s.Products);
                }
            }

            var supplier = await query.FirstOrDefaultAsync(s => s.Id == id);

            if (supplier is null || supplier.IsDeleted)
                return new ErrorResultWithData<Supplier>($"There is no supplier with ID {id}");

            return new SuccessResultWithData<Supplier>("Supplier found.", supplier);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<Supplier>(ex.Message);
        }
    }
    public override Task<IServiceResultWithData<IEnumerable<Supplier>>> GetAllAsync()
    {
        throw new Exception("Use GetAllSuppliersWithIncludeAsync instead.");
    }
}