namespace EventFlow.Data.Entities;

public class Tarjeta
{
    public int Id { get; set; }
    public string NumeroTarjeta { get; set; } = string.Empty;
    public decimal SaldoDisponible { get; set; }
    public DateTime FechaExpiracion { get; set; }
    public bool EstaActiva { get; set; } = true;
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
