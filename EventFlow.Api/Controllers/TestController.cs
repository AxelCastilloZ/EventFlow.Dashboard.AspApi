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
        
        // Get updated dashboard statistics from database
        var dashboardData = await _dashboardService.GetDashboardDataAsync();

        // Send updated data to all connected clients via SignalR
        await _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate", dashboardData);

        // Return the updated dashboard data
        return Ok(new
        {
            message = "Dashboard data refreshed and sent via SignalR",
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