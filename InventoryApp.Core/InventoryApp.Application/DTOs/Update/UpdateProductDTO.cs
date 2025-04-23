namespace InventoryApp.Application.DTOs.Update;

public class UpdateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}