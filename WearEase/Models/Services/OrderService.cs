
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using WearEase.Hubs;
using WearEase.Models;
using WearEase.Models.Interfaces;
using WearEase.Models.ViewModels;

namespace WearEase.Models.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo, IHubContext<OrderHub> hubContext, UserManager<IdentityUser> userManager)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            try
            {
                Logger.LogMessage($"Fetching order with ID {id}");
                return await _orderRepo.GetOrderByIdAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            try
            {
                Logger.LogMessage($"Fetching orders for customer {customerId}");
                return await _orderRepo.GetOrdersByCustomerAsync(customerId);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<int> PlaceOrderAsync(string customerId,  string paymentMethod)
        {
            try
            {
                Logger.LogMessage($"Placing order for customer {customerId}");

                var cart = await _cartRepo.GetCartByCustomerAsync(customerId);

                if (cart == null || cart.CartItems.Count == 0)
                {
                    Logger.LogMessage("Order failed: Cart is empty.");
                    return 0;
                }

                decimal total = cart.CartItems.Sum(i => i.Quantity * i.Product.Price);

                
                 var order = new Order
                 {
                     
                     CustomerId = customerId,
                     OrderDate = DateTime.Now,
                     TotalAmount = total,
                     Status = "Pending",   //  VERY IMPORTANT
                     PaymentMethod = paymentMethod,
                     PaymentStatus = paymentMethod == "Card" ? "Paid" : "Pending"
                 };
 
               

                int orderId = await _orderRepo.CreateOrderAsync(order);

                foreach (var item in cart.CartItems)
                {
                    await _orderRepo.AddOrderItemAsync(new OrderItem
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    });
                }


                await _cartRepo.ClearCartAsync(cart.Id);
                await _hubContext.Clients.All.SendAsync(
    "ReceiveOrderNotification",
    $"🛒 New order placed! Order ID: {orderId}"
);

                Logger.LogMessage($"Order placed successfully. OrderId {orderId}, Total {total}");
                return orderId;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                Logger.LogMessage("Fetching all orders");
                return await _orderRepo.GetAllOrdersAsync();
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
        public async Task ApproveOrderAsync(int orderId)
        {
            await _orderRepo.UpdateOrderStatusAsync(orderId, "Approved");
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            await _orderRepo.DeleteOrderAsync(orderId);
        }
        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            return await _orderRepo.GetDashboardStatsAsync();
        }
        public async Task MarkDeliveredAsync(int orderId)
        {
            await _orderRepo.UpdateOrderStatusAsync(orderId, "Delivered");
        }
        public async Task<List<(DateTime Date, int Count)>> GetOrdersPerDayAsync()
        {
            return await _orderRepo.GetOrdersPerDayAsync();
        }

    }
}
