namespace EventFlow.DTOs;

public class EventoPagoDto
{
    public int IdCompra { get; set; }  // ← int, no string (era el error)
    public string Estado { get; set; } = string.Empty;  // ← Requerido
    public string? Mensaje { get; set; }  // ← Agregar este campo nuevo
    public DateTime Timestamp { get; set; }

    // Campos opcionales (por si los envían)
    public decimal? MontoTotal { get; set; }
    public int? UsuarioId { get; set; }
    public int? ProductoId { get; set; }
    public int? TarjetaId { get; set; }
}