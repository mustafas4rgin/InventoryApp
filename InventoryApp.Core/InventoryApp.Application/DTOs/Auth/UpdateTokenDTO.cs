namespace InventoryApp.Application.DTOs;

public class UpdateTokenDTO 
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
}