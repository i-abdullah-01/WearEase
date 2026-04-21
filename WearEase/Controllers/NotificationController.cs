/*using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Homework09.Hubs;
namespace Homework09.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            return Ok();
        }
    }
}
*/