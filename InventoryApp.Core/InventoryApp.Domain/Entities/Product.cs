namespace InventoryApp.Domain.Entities;

public class Product : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    //navigation properties
    public Supplier Supplier { get; set; } = null!;
    public Category Category { get; set; } = null!;
}