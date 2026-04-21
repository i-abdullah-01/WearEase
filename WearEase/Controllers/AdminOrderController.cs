using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminOrderController : Controller
    {
        private readonly OrderService _orderService;

        public AdminOrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult  Index1()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            await _orderService.ApproveOrderAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Deliver(int id)
        {
            await _orderService.MarkDeliveredAsync(id);
            return RedirectToAction("Index");
        }

    }
}
