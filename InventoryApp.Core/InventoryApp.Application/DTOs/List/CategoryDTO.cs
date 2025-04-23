namespace InventoryApp.Application.DTOs.List;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ProductDTO> Products { get; set; } = null!;
}