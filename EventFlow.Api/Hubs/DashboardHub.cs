using Microsoft.AspNetCore.SignalR;

namespace EventFlow.Api.Hubs;

public class DashboardHub : Hub
{
    public async Task SendDashboardUpdate(object data)
    {
        await Clients.All.SendAsync("ReceiveDashboardUpdate", data);
    }
}
