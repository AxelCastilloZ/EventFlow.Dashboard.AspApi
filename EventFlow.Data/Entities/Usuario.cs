namespace EventFlow.Data.Entities;

public class User
{
    public int User_Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string First_Name { get; set; } = string.Empty;
    public string Last_Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
