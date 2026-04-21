using WearEase.Models;
using WearEase.Models.ViewModels;

namespace WearEase.Models.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(Order order);
        Task AddOrderItemAsync(OrderItem item);

        Task<Order?> GetOrderByIdAsync(int id);
        Task<List<OrderItem>> GetOrderItemsAsync(int orderId);

        Task<List<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<List<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task DeleteOrderAsync(int orderId);
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
        Task<List<(DateTime Date, int Count)>> GetOrdersPerDayAsync();



    }
}
