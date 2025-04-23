namespace InventoryApp.Domain.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = null!;
}