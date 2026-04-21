using Microsoft.AspNetCore.SignalR;

namespace WearEase.Hubs
{
    public class OrderHub : Hub
    {
        public async Task NotifyNewOrder(string message)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", message);
        }
    }
}
