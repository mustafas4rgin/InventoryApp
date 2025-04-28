namespace InventoryApp.Domain.Entities;

public class User : EntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int SupplierId { get; set; }
    public bool IsApproved { get; set; } = false;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    //navigation properties
    public ICollection<Notification> Notifications { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public Role Role { get; set; } = null!;
}