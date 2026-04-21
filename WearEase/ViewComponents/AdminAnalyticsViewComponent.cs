using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;
using WearEase.Models.ViewModels;
namespace WearEase.ViewComponents
{
    public class AdminAnalyticsViewComponent :ViewComponent
    {

        private readonly OrderService _orderService;

        public AdminAnalyticsViewComponent(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();

            var vm = new AdminAnalyticsViewModel
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders
                    .Where(o => o.Status == "Approved" || o.Status == "Delivered")
                    .Sum(o => o.TotalAmount),

                ApprovedOrders = orders.Count(o => o.Status == "Approved"),
                PendingOrders = orders.Count(o => o.Status == "Pending"),
                DeliveredOrders = orders.Count(o => o.Status == "Delivered")
            };

            return View(vm);
        }

    }
}
