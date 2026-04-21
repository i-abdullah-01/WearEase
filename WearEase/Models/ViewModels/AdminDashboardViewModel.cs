using System.Collections.Generic;
namespace WearEase.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Order> Orders { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }
        public int ApprovedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public List<string> OrderDates { get; set; } = new();
        public List<int> OrdersPerDay { get; set; } = new();


    }
}
