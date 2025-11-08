namespace EventFlow.Data.Entities;

public class PurchaseDetail
{
    public int Purchase_Detail_Id { get; set; }
    public int Purchase_Id { get; set; }
    public int Product_Id { get; set; }
    public int Quantity { get; set; }
    public int Total { get; set; }
    public DateTime Purchase_Date { get; set; }
    
    // Navigation properties
    public Purchase? Purchase { get; set; }
    public Product? Product { get; set; }
}

