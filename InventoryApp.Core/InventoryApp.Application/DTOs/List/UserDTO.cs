public class UserDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string RoleName { get; set; } 
    public string SupplierName { get; set; } 
    public bool IsApproved { get; set; }
}
