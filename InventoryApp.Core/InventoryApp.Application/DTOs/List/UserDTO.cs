namespace InventoryApp.Application.DTOs.List;

public class UserDTO
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    //navigation properties
    public List<NotificationDTO> Notifications { get; set; } = null!;
    public SupplierDTO Supplier { get; set; } = null!;
    public RoleDTO Role { get; set; } = null!;
}