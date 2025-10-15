namespace EventFlow.DTOs;

public class DashboardDataDto
{
    public int TotalCompras { get; set; }
    public decimal ValorTotalTransaccionado { get; set; }
    public decimal SaldoPromedioTarjetas { get; set; }
    public List<ProductoVendidoDto> ProductosMasVendidos { get; set; } = new();
    public List<ProductoStockDto> ProductosStockBajo { get; set; } = new();
    public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;
}
