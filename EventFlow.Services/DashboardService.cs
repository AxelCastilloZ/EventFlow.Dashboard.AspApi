using EventFlow.Data;
using EventFlow.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Services;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        var dashboardData = new DashboardDataDto
        {
            // Total purchases (all purchases are considered successful in Grupo3 schema)
            TotalCompras = await _context.Purchases.CountAsync(),
            
            // Total transaction value (sum of all purchase subtotals)
            ValorTotalTransaccionado = await _context.Purchases
                .SumAsync(p => (decimal)p.SubTotal),
            
            // Average card balance (Money field from Cards table)
            // Note: Cards don't have EstaActiva field, so we use all cards
            SaldoPromedioTarjetas = await _context.Cards
                .AverageAsync(c => (decimal?)c.Money) ?? 0,
            
            ProductosMasVendidos = await GetProductosMasVendidosAsync(),
            ProductosStockBajo = await GetProductosStockBajoAsync(),
            UltimaActualizacion = DateTime.UtcNow
        };

        return dashboardData;
    }

    private async Task<List<ProductoVendidoDto>> GetProductosMasVendidosAsync()
    {
        // Products are sold through PurchaseDetails, not directly from Purchases
        // We need to group by Product from PurchaseDetails
        return await _context.PurchaseDetails
            .Include(pd => pd.Product)
            .GroupBy(pd => new { pd.Product_Id, pd.Product!.Product_Name })
            .Select(g => new ProductoVendidoDto
            {
                ProductoId = g.Key.Product_Id,
                NombreProducto = g.Key.Product_Name,
                CantidadVendida = g.Sum(pd => pd.Quantity),
                MontoTotal = g.Sum(pd => (decimal)pd.Total)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .Take(5)
            .ToListAsync();
    }

    private async Task<List<ProductoStockDto>> GetProductosStockBajoAsync()
    {
        // Products use Quantity field instead of Stock
        return await _context.Products
            .Where(p => p.Quantity <= 10)
            .OrderBy(p => p.Quantity)
            .Select(p => new ProductoStockDto
            {
                ProductoId = p.Product_Id,
                NombreProducto = p.Product_Name,
                StockActual = p.Quantity
            })
            .Take(10)
            .ToListAsync();
    }
}
