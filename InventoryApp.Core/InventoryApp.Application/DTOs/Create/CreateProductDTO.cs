namespace InventoryApp.Application.DTOs.Create;

public class CreateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}