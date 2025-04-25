namespace InventoryApp.Domain.Entities;

public class AccessToken : EntityBase
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    //navigation properties
    public User User { get; set; } = null!;
}