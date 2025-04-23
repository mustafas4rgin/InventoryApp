
namespace InventoryApp.Application.DTOs.List;

public class ProductDTO
{
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    //navigation properties
    public SupplierDTO Supplier { get; set; } = null!;
    public CategoryDTO Category { get; set; } = null!;
}