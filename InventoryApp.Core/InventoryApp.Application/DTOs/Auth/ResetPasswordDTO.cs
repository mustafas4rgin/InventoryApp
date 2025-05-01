namespace InventoryApp.Application.DTOs;

public class ResetPasswordDTO
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}