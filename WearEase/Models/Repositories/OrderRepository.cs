/*using Microsoft.EntityFrameworkCore;
using WearEase.Data;
using WearEase.Models;
using WearEase.Models.Interfaces;
namespace WearEase.Models.Repositories
{
    public class OrderRepository:IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        public Order GetOrderById(int id)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefault(o => o.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Order> GetOrdersByCustomer(string customerId)
        {
            try
            {
                return _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.OrderItems)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Order>();
            }
        }

        public void AddOrderItem(OrderItem item)
        {
            try
            {
                _context.OrderItems.Add(item);
                _context.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            try
            {
                return _context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .Include(oi => oi.Product)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<OrderItem>();
            }
        }
        public List<Order> GetAllOrders()
        {
            try
            {
                return _context.Orders
                          .Include(o => o.OrderItems)
                          .ThenInclude(i => i.Product)
                          .ToList();
            }
            catch
            {
                return new List<Order>();
            }
        }

    }
}
*/
/*using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<int> CreateOrderAsync(Order order)
        {
            using var connection = CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Orders (CustomerId, OrderDate, TotalAmount)
                  OUTPUT INSERTED.Id
                  VALUES (@CustomerId, @OrderDate, @TotalAmount)", order);
        }

        public async Task AddOrderItemAsync(OrderItem item)
        {
            using var connection = CreateConnection();

            await connection.ExecuteAsync(
                @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                  VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)", item);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            using var connection = CreateConnection();

            var order = await connection.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM Orders WHERE Id=@Id", new { Id = id });

            if (order == null) return null;

            order.OrderItems = await GetOrderItemsAsync(id);
            return order;
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT oi.*, p.*
                FROM OrderItems oi
                INNER JOIN Products p ON oi.ProductId = p.Id
                WHERE oi.OrderId = @OrderId";

            var lookup = new Dictionary<int, OrderItem>();

            await connection.QueryAsync<OrderItem, Product, OrderItem>(
                sql,
                (item, product) =>
                {
                    item.Product = product;
                    lookup[item.Id] = item;
                    return item;
                },
                new { OrderId = orderId },
                splitOn: "Id"
            );

            return lookup.Values.ToList();
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            using var connection = CreateConnection();

            var orders = (await connection.QueryAsync<Order>(
                "SELECT * FROM Orders WHERE CustomerId=@CustomerId",
                new { CustomerId = customerId })).ToList();

            foreach (var order in orders)
                order.OrderItems = await GetOrderItemsAsync(order.Id);

            return orders;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            using var connection = CreateConnection();

            var orders = (await connection.QueryAsync<Order>(
                "SELECT * FROM Orders")).ToList();

            foreach (var order in orders)
                order.OrderItems = await GetOrderItemsAsync(order.Id);

            return orders;
        }
    }
}
*/
/*using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<int> CreateOrderAsync(Order order)
        {
            using var connection = CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Orders (CustomerId, OrderDate, TotalAmount)
                  OUTPUT INSERTED.Id
                  VALUES (@CustomerId, @OrderDate, @TotalAmount)", order);
        }

        public async Task AddOrderItemAsync(OrderItem item)
        {
            using var connection = CreateConnection();

            await connection.ExecuteAsync(
                @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                  VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)", item);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            using var connection = CreateConnection();

            var order = await connection.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM Orders WHERE Id=@Id",
                new { Id = id });

            if (order == null)
                return null;

            order.OrderItems = await GetOrderItemsAsync(id);
            return order;
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT oi.*, p.*
                FROM OrderItems oi
                INNER JOIN Products p ON oi.ProductId = p.Id
                WHERE oi.OrderId = @OrderId";

            var lookup = new Dictionary<int, OrderItem>();

            await connection.QueryAsync<OrderItem, Product, OrderItem>(
                sql,
                (item, product) =>
                {
                    item.Product = product;
                    lookup[item.Id] = item;
                    return item;
                },
                new { OrderId = orderId },
                splitOn: "Id"
            );

            return lookup.Values.ToList();
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            using var connection = CreateConnection();

            var orders = (await connection.QueryAsync<Order>(
                "SELECT * FROM Orders WHERE CustomerId=@CustomerId",
                new { CustomerId = customerId })).ToList();

            foreach (var order in orders)
            {
                order.OrderItems = await GetOrderItemsAsync(order.Id);
            }

            return orders;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            using var connection = CreateConnection();

            var orders = (await connection.QueryAsync<Order>(
                "SELECT * FROM Orders")).ToList();

            foreach (var order in orders)
            {
                order.OrderItems = await GetOrderItemsAsync(order.Id);
            }

            return orders;
        }
    }
}
*/
/*using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<int> CreateOrderAsync(Order order)
        {
            _logger.LogInformation("Creating order for CustomerId={CustomerId}", order.CustomerId);

            try
            {
                using var connection = CreateConnection();

                int newId = await connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Orders (CustomerId, OrderDate, TotalAmount)
                      OUTPUT INSERTED.Id
                      VALUES (@CustomerId, @OrderDate, @TotalAmount)",
                    order);

                _logger.LogInformation("Order created successfully with Id={OrderId}", newId);
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for CustomerId={CustomerId}", order.CustomerId);
                throw;
            }
        }

        public async Task AddOrderItemAsync(OrderItem item)
        {
            _logger.LogInformation("Adding OrderItem for OrderId={OrderId}, ProductId={ProductId}", item.OrderId, item.ProductId);

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                      VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)", item);

                _logger.LogInformation("OrderItem added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add OrderItem for OrderId={OrderId}", item.OrderId);
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Order with Id={OrderId}", id);

            try
            {
                using var connection = CreateConnection();

                var order = await connection.QueryFirstOrDefaultAsync<Order>(
                    "SELECT * FROM Orders WHERE Id=@Id",
                    new { Id = id });

                if (order == null)
                {
                    _logger.LogWarning("Order not found: Id={OrderId}", id);
                    return null;
                }

                order.OrderItems = await GetOrderItemsAsync(id);

                _logger.LogInformation("Order fetched successfully.");
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Order with Id={OrderId}", id);
                throw;
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            _logger.LogDebug("Fetching OrderItems for OrderId={OrderId}", orderId);

            try
            {
                using var connection = CreateConnection();

                var sql = @"
                    SELECT oi.*, p.*
                    FROM OrderItems oi
                    INNER JOIN Products p ON oi.ProductId = p.Id
                    WHERE oi.OrderId = @OrderId";

                var lookup = new Dictionary<int, OrderItem>();

                await connection.QueryAsync<OrderItem, Product, OrderItem>(
                    sql,
                    (item, product) =>
                    {
                        item.Product = product;
                        lookup[item.Id] = item;
                        return item;
                    },
                    new { OrderId = orderId },
                    splitOn: "Id"
                );

                _logger.LogDebug("Fetched {Count} OrderItems.", lookup.Count);

                return lookup.Values.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching OrderItems for OrderId={OrderId}", orderId);
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            _logger.LogInformation("Fetching all orders for CustomerId={CustomerId}", customerId);

            try
            {
                using var connection = CreateConnection();

                var orders = (await connection.QueryAsync<Order>(
                    "SELECT * FROM Orders WHERE CustomerId=@CustomerId",
                    new { CustomerId = customerId })).ToList();

                foreach (var order in orders)
                {
                    order.OrderItems = await GetOrderItemsAsync(order.Id);
                }

                _logger.LogInformation("Fetched {Count} orders for CustomerId={CustomerId}", orders.Count, customerId);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for CustomerId={CustomerId}", customerId);
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Fetching ALL orders.");

            try
            {
                using var connection = CreateConnection();

                var orders = (await connection.QueryAsync<Order>(
                    "SELECT * FROM Orders")).ToList();

                foreach (var order in orders)
                {
                    order.OrderItems = await GetOrderItemsAsync(order.Id);
                }

                _logger.LogInformation("Fetched {Count} total orders.", orders.Count);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders");
                throw;
            }
        }
    }
}
*/
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WearEase.Models;
using WearEase.Models.Interfaces;
using WearEase.Models.Services; 
using WearEase.Models.ViewModels;

namespace WearEase.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<int> CreateOrderAsync(Order order)
        {
            Logger.LogMessage($"Creating order for CustomerId={order.CustomerId}");

            try
            {
                using var connection = CreateConnection();

                int newId = await connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Orders (CustomerId, OrderDate, TotalAmount)
                      OUTPUT INSERTED.Id
                      VALUES (@CustomerId, @OrderDate, @TotalAmount)",
                    order);

                Logger.LogMessage($"Order created successfully. OrderId={newId}");
                return newId;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task AddOrderItemAsync(OrderItem item)
        {
            Logger.LogMessage(
                $"Adding OrderItem. OrderId={item.OrderId}, ProductId={item.ProductId}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                      VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)",
                    item);

                Logger.LogMessage("OrderItem added successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            Logger.LogMessage($"Fetching order. OrderId={id}");

            try
            {
                using var connection = CreateConnection();

                var order = await connection.QueryFirstOrDefaultAsync<Order>(
                    "SELECT * FROM Orders WHERE Id=@Id",
                    new { Id = id });

                if (order == null)
                {
                    Logger.LogMessage($"Order not found. OrderId={id}");
                    return null;
                }

                order.OrderItems = await GetOrderItemsAsync(id);

                Logger.LogMessage($"Order fetched successfully. OrderId={id}");
                return order;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            Logger.LogMessage($"Fetching OrderItems for OrderId={orderId}");

            try
            {
                using var connection = CreateConnection();

                var sql = @"
                    SELECT oi.*, p.*
                    FROM OrderItems oi
                    INNER JOIN Products p ON oi.ProductId = p.Id
                    WHERE oi.OrderId = @OrderId";

                var lookup = new Dictionary<int, OrderItem>();

                await connection.QueryAsync<OrderItem, Product, OrderItem>(
                    sql,
                    (item, product) =>
                    {
                        item.Product = product;
                        lookup[item.Id] = item;
                        return item;
                    },
                    new { OrderId = orderId },
                    splitOn: "Id"
                );

                Logger.LogMessage(
                    $"Fetched {lookup.Count} OrderItems for OrderId={orderId}");

                return lookup.Values.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            Logger.LogMessage($"Fetching orders for CustomerId={customerId}");

            try
            {
                using var connection = CreateConnection();

                var orders = (await connection.QueryAsync<Order>(
                    "SELECT * FROM Orders WHERE CustomerId=@CustomerId",
                    new { CustomerId = customerId }))
                    .ToList();

                foreach (var order in orders)
                {
                    order.OrderItems = await GetOrderItemsAsync(order.Id);
                }

                Logger.LogMessage(
                    $"Fetched {orders.Count} orders for CustomerId={customerId}");

                return orders;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            Logger.LogMessage("Fetching ALL orders.");

            try
            {
                using var connection = CreateConnection();

                var orders = (await connection.QueryAsync<Order>(
                    "SELECT * FROM Orders"))
                    .ToList();

                foreach (var order in orders)
                {
                    order.OrderItems = await GetOrderItemsAsync(order.Id);
                }

                Logger.LogMessage($"Fetched total {orders.Count} orders.");
                return orders;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Orders SET Status=@Status WHERE Id=@Id",
                new { Status = status, Id = orderId }
            );
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            using var connection = CreateConnection();

            await connection.ExecuteAsync("DELETE FROM OrderItems WHERE OrderId=@Id", new { Id = orderId });
            await connection.ExecuteAsync("DELETE FROM Orders WHERE Id=@Id", new { Id = orderId });
        }
        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            using var connection = CreateConnection();

            var sql = @"
        SELECT
            COUNT(*) AS TotalOrders,
            ISNULL(SUM(TotalAmount),0) AS TotalRevenue,
            SUM(CASE WHEN Status='Pending' THEN 1 ELSE 0 END) AS PendingOrders,
            SUM(CASE WHEN Status='Approved' THEN 1 ELSE 0 END) AS ApprovedOrders,
            SUM(CASE WHEN Status='Delivered' THEN 1 ELSE 0 END) AS DeliveredOrders
        FROM Orders";

            return await connection.QueryFirstAsync<AdminDashboardViewModel>(sql);
        }
        public async Task<List<(DateTime Date, int Count)>> GetOrdersPerDayAsync()
        {
            using var connection = CreateConnection();

            var sql = @"
        SELECT CAST(OrderDate AS DATE) AS OrderDate,
               COUNT(*) AS TotalOrders
        FROM Orders
        GROUP BY CAST(OrderDate AS DATE)
        ORDER BY OrderDate";

            var result = await connection.QueryAsync(sql);

            return result.Select(r => (
                Date: (DateTime)r.OrderDate,
                Count: (int)r.TotalOrders
            )).ToList();
        }



    }
}
