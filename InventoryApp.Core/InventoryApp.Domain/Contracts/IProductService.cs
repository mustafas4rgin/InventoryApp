using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface IProductService : IGenericService<Product>
{
    Task<IServiceResultWithData<IEnumerable<Product>>> GetProductsWithIncludeAsync(string? include);
    Task<IServiceResultWithData<Product>> GetProductByIdWithIncludeAsync(string? include, int id);
}