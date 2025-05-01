using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface IProductService : IGenericService<Product>
{
    Task<IServiceResultWithData<IEnumerable<Product>>> GetProductsWithIncludeAsync(string? include, string? search);
    Task<IServiceResultWithData<Product>> GetProductByIdWithIncludeAsync(string? include, int id);
    Task<IServiceResultWithData<IEnumerable<Product>>> GetProductsBySupplierIdAsync(int supplierId);
    Task<IServiceResultWithData<IEnumerable<Product>>> GetDeletedProductsBySupplierIdAsync(int userId);

}