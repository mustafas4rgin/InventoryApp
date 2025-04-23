using System.ComponentModel;

namespace InventoryApp.Application.DTOs.Create;

public class CreateUserDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordMatch { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int SupplierId { get; set; }
}