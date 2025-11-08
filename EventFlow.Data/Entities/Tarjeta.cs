namespace EventFlow.Data.Entities;

public class Card
{
    public int Card_Id { get; set; }
    public int User_Id { get; set; }
    public string Card_Type { get; set; } = string.Empty;
    public string Card_Number { get; set; } = string.Empty;
    public int Money { get; set; }
    public DateTime Expiration_Date { get; set; }
    
    // Navigation properties
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
