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
            TotalCompras = await _context.Compras.CountAsync(c => c.EsExitosa),
            ValorTotalTransaccionado = await _context.Compras
                .Where(c => c.EsExitosa)
                .SumAsync(c => c.MontoTotal),
            SaldoPromedioTarjetas = await _context.Tarjetas
                .Where(t => t.EstaActiva)
                .AverageAsync(t => (decimal?)t.SaldoDisponible) ?? 0,
            ProductosMasVendidos = await GetProductosMasVendidosAsync(),
            ProductosStockBajo = await GetProductosStockBajoAsync(),
            UltimaActualizacion = DateTime.UtcNow
        };

        return dashboardData;
    }

    private async Task<List<ProductoVendidoDto>> GetProductosMasVendidosAsync()
    {
        return await _context.Compras
            .Where(c => c.EsExitosa)
            .GroupBy(c => new { c.ProductoId, c.Producto.Nombre })
            .Select(g => new ProductoVendidoDto
            {
                ProductoId = g.Key.ProductoId,
                NombreProducto = g.Key.Nombre,
                CantidadVendida = g.Sum(c => c.Cantidad),
                MontoTotal = g.Sum(c => c.MontoTotal)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .Take(5)
            .ToListAsync();
    }

    private async Task<List<ProductoStockDto>> GetProductosStockBajoAsync()
    {
        return await _context.Productos
            .Where(p => p.Stock <= 10)
            .OrderBy(p => p.Stock)
            .Select(p => new ProductoStockDto
            {
                ProductoId = p.Id,
                NombreProducto = p.Nombre,
                StockActual = p.Stock
            })
            .Take(10)
            .ToListAsync();
    }
}
