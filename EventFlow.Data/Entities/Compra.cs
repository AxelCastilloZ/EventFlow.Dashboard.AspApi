namespace EventFlow.Data.Entities;

public class Purchase
{
    public int Purchase_Id { get; set; }
    public int Card_Id { get; set; }
    public int User_Id { get; set; }
    public int SubTotal { get; set; }
    public DateTime Purchase_Date { get; set; }
    
    // Navigation properties
    public Card? Card { get; set; }
    public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
}
