namespace EventFlow.Data.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    
    public ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
