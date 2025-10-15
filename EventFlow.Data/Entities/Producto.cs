namespace EventFlow.Data.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? Categoria { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
