using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface ISupplierService : IGenericService<Supplier>
{
    Task<IServiceResultWithData<IEnumerable<Supplier>>> GetAllSuppliersWithIncludeAsync(string? include,string? search);
    Task<IServiceResultWithData<Supplier>> GetSupplierByIdWithIncludeAsync(string? include, int id);
}