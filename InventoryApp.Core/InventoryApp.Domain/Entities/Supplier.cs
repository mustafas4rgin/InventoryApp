namespace InventoryApp.Domain.Entities;

public class Supplier : EntityBase
{
    public string Name { get; set; } = string.Empty;
    //navigation properties
    public ICollection<Product> Products { get; set; } = null!;
    public ICollection<User> Users { get; set; } = null!;
}