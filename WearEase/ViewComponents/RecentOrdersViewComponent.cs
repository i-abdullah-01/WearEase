using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;
namespace WearEase.ViewComponents
{
    public class RecentOrdersViewComponent : ViewComponent
    {
        private readonly OrderService _orderService;

        public RecentOrdersViewComponent(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 5)
        {
            var orders = (await _orderService.GetAllOrdersAsync())
                            .OrderByDescending(o => o.OrderDate)
                            .Take(count)
                            .ToList();

            return View(orders);
        }
    }
}
