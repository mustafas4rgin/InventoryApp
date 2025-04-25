using InventoryApp.Application.DTOs.List;

namespace InventoryApp.Application.DTOs;

public class TokenDTO
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public UserDTO User { get; set; } = null!;
}