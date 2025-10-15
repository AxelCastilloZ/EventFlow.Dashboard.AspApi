using EventFlow.Api.Hubs;
using EventFlow.Data;
using EventFlow.Data.Entities;
using EventFlow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        // 1. Crear una nueva compra aleatoria
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
}