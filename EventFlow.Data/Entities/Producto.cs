namespace EventFlow.Data.Entities;

public class Product
{
    public int Product_Id { get; set; }
    public string Product_Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Price { get; set; }
    
    // Navigation properties
    public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
}
