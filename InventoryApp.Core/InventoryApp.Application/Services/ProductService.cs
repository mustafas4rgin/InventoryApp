using FluentValidation;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace InventoryApp.Application.Services;

public class ProductService : GenericService<Product>, IProductService
{
    private readonly IGenericRepository _genericRepository;
    public ProductService(
        IGenericRepository genericRepository,
        IValidator<Product> validator
    ) : base(genericRepository,validator) 
    {
        _genericRepository = genericRepository;
    }
    public async Task<IServiceResultWithData<IEnumerable<Product>>> GetDeletedProductsBySupplierIdAsync(int userId)
    {
        try
        {
            var user = await _genericRepository.GetAll<User>().FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
                return new ErrorResultWithData<IEnumerable<Product>>($"There is no user with ID : {userId}");

            var deletedProducts = await _genericRepository.GetAll<Product>()
                                .Where(p => p.SupplierId == user.SupplierId && p.IsDeleted)
                                .Include(p => p.Category)
                                .ToListAsync();
            
            return new SuccessResultWithData<IEnumerable<Product>>("Products found.",deletedProducts);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<Product>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<Product>>> GetProductsBySupplierIdAsync(int userId)
    {
        try
        {
            var user = await _genericRepository.GetAll<User>().FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
                return new ErrorResultWithData<IEnumerable<Product>>($"There is no user with ID : {userId}");

            var products = await _genericRepository.GetAll<Product>()
                                .Where(p => p.SupplierId == user.SupplierId && !p.IsDeleted)
                                .Include(p => p.Category)
                                .ToListAsync();

            if (!products.Any())
                return new ErrorResultWithData<IEnumerable<Product>>("This supplier has no products.");

            return new SuccessResultWithData<IEnumerable<Product>>("Products found.",products);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<Product>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<Product>>> GetProductsWithIncludeAsync(string? include, string? search)
    {
        try
        {
            var query = _genericRepository.GetAll<Product>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "category")
                        query = query.Include(p => p.Category);
                    else if (inc == "supplier")
                        query = query.Include(p => p.Supplier);
                }
            }

            var products = await query.Where(s => !s.IsDeleted)
                                .Where(p => string.IsNullOrEmpty(search) || p.Name.ToLower().Contains(search.ToLower()))
                                .ToListAsync();

            if (!products.Any())
                return new ErrorResultWithData<IEnumerable<Product>>("There is no product.");

            return new SuccessResultWithData<IEnumerable<Product>>("Products found.", products);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<Product>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<Product>> GetProductByIdWithIncludeAsync(string? include, int id)
    {
        try
        {
            var query = _genericRepository.GetAll<Product>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "category")
                        query = query.Include(p => p.Category);
                    else if (inc == "supplier")
                        query = query.Include(p => p.Supplier);
                }
            }

            var product = await query.Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
                return new ErrorResultWithData<Product>($"There is no product with ID : {id}");

            return new SuccessResultWithData<Product>("Product found.", product);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<Product>(ex.Message);
        }
    }
}