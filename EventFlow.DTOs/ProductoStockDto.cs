namespace EventFlow.DTOs;

public class ProductoStockDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int StockActual { get; set; }
}
