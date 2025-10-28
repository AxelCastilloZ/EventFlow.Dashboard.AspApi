using EventFlow.Api.Hubs;
using EventFlow.Data;
using EventFlow.Data.Entities;
using EventFlow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly AppDbContext _context;

    public TestController(DashboardService dashboardService, IHubContext<DashboardHub> hubContext, AppDbContext context)
    {
        _dashboardService = dashboardService;
        _hubContext = hubContext;
        _context = context;
    }

    [HttpPost("simular-evento-pago")]
    public async Task<IActionResult> SimularEventoPago()
    {
        // 1. Asegurar que existen datos de prueba
        await EnsureSampleDataExistsAsync();

        // 2. Crear una nueva compra aleatoria
        var random = new Random();

        var nuevaCompra = new Compra
        {
            UsuarioId = random.Next(1, 4),
            ProductoId = random.Next(1, 6),
            TarjetaId = random.Next(1, 4),
            Cantidad = random.Next(1, 4),
            MontoTotal = (decimal)(random.NextDouble() * 500 + 50),
            FechaHora = DateTime.UtcNow,
            EsExitosa = true
        };

        _context.Compras.Add(nuevaCompra);
        await _context.SaveChangesAsync();

        // 2. Obtener las estadísticas actualizadas
        var dashboardData = await _dashboardService.GetDashboardDataAsync();

        // 3. Enviar los datos a todos los clientes conectados vía SignalR
        await _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate", dashboardData);

        // 4. Retornar los datos también como respuesta HTTP
        return Ok(new
        {
            message = "Evento procesado y datos enviados vía SignalR",
            data = dashboardData
        });
    }

    [HttpGet("dashboard-data")]
    public async Task<IActionResult> GetDashboardData()
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync();
        return Ok(dashboardData);
    }

    private async Task EnsureSampleDataExistsAsync()
    {
        // Check if sample data already exists
        if (await _context.Productos.AnyAsync()) return;

        // Create sample products
        var productos = new[]
        {
            new Producto { Nombre = "Laptop Gaming", Precio = 1299.99m, Stock = 15, Categoria = "Electronics" },
            new Producto { Nombre = "Smartphone Pro", Precio = 899.99m, Stock = 25, Categoria = "Electronics" },
            new Producto { Nombre = "Wireless Headphones", Precio = 199.99m, Stock = 8, Categoria = "Audio" },
            new Producto { Nombre = "Coffee Maker", Precio = 79.99m, Stock = 12, Categoria = "Kitchen" },
            new Producto { Nombre = "Running Shoes", Precio = 129.99m, Stock = 5, Categoria = "Sports" }
        };
        _context.Productos.AddRange(productos);

        // Create sample users
        var usuarios = new[]
        {
            new Usuario { Nombre = "Juan Pérez", Email = "juan@test.com" },
            new Usuario { Nombre = "María García", Email = "maria@test.com" },
            new Usuario { Nombre = "Carlos López", Email = "carlos@test.com" }
        };
        _context.Usuarios.AddRange(usuarios);

        // Save to get IDs
        await _context.SaveChangesAsync();

        // Create sample cards for each user
        var tarjetas = new[]
        {
            new Tarjeta { NumeroTarjeta = "****1234", SaldoDisponible = 1500.00m, FechaExpiracion = DateTime.Now.AddYears(2), UsuarioId = 1 },
            new Tarjeta { NumeroTarjeta = "****5678", SaldoDisponible = 850.00m, FechaExpiracion = DateTime.Now.AddYears(3), UsuarioId = 2 },
            new Tarjeta { NumeroTarjeta = "****9012", SaldoDisponible = 2200.00m, FechaExpiracion = DateTime.Now.AddYears(1), UsuarioId = 3 }
        };
        _context.Tarjetas.AddRange(tarjetas);

        await _context.SaveChangesAsync();
    }
}