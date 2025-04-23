namespace InventoryApp.Application.DTOs.List;

public class SupplierDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    //navigation properties
    public List<ProductDTO> Products { get; set; } = null!;
    public List<UserDTO> Users { get; set; } = null!;
}