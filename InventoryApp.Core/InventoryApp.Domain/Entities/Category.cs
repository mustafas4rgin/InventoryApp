namespace InventoryApp.Domain.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;
    //navigation properties
    public ICollection<Product> Products { get; set; } = null!;
}