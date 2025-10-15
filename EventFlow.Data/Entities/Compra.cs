namespace EventFlow.Data.Entities;

public class Compra
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal MontoTotal { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public bool EsExitosa { get; set; } = true;
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    
    public int TarjetaId { get; set; }
    public Tarjeta Tarjeta { get; set; } = null!;
}
