namespace EventFlow.DTOs;


public class EventoPagoDto
{
    // CAMPOS MÍNIMOS 
    public string? IdCompra { get; set; }
    public string? Estado { get; set; }
    public DateTime Timestamp { get; set; }

    public decimal? MontoTotal { get; set; }
    public int? UsuarioId { get; set; }
    public int? ProductoId { get; set; }
    public int? TarjetaId { get; set; }
}