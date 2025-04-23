namespace InventoryApp.Application.DTOs.Update;

public class UpdateUserDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int SupplierId { get; set; }
}