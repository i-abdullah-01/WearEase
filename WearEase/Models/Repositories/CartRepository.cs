using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WearEase.Models;
using WearEase.Models.Interfaces;
using WearEase.Models.Services;

namespace WearEase.Models.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public CartRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<Cart?> GetCartByCustomerAsync(string customerId)
        {
            Logger.LogMessage($"Fetching cart for customer {customerId}");

            try
            {
                using var connection = CreateConnection();

                var cart = await connection.QueryFirstOrDefaultAsync<Cart>(
                    "SELECT * FROM Carts WHERE CustomerId=@CustomerId",
                    new { CustomerId = customerId });

                if (cart == null)
                {
                    Logger.LogMessage($"No cart found for customer {customerId}");
                    return null;
                }

                var items = await GetCartItemsAsync(cart.Id);
                cart.CartItems = items.ToList();

                Logger.LogMessage($"Cart {cart.Id} loaded with {cart.CartItems.Count} items");

                return cart;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task CreateCartAsync(Cart cart)
        {
            Logger.LogMessage($"Creating cart for customer {cart.CustomerId}");

            try
            {
                using var connection = CreateConnection();

                var id = await connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Carts (CustomerId)
                      OUTPUT INSERTED.Id
                      VALUES (@CustomerId)",
                    cart);

                cart.Id = id;

                Logger.LogMessage($"Cart created successfully. CartId={id}");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId)
        {
            Logger.LogMessage($"Fetching items for cart {cartId}");

            try
            {
                using var connection = CreateConnection();

                var sql = @"
                    /*SELECT ci.*, p.*
                    FROM CartItems ci
                    INNER JOIN Products p ON ci.ProductId = p.Id
                    WHERE ci.CartId = @CartId*/
SELECT 
    ci.Id,
    ci.CartId,
    ci.ProductId,
    ci.Quantity,
    p.Id AS ProductIdSplit,
    p.Name,
    p.Price,
    p.ImageUrl
FROM CartItems ci
INNER JOIN Products p ON ci.ProductId = p.Id
WHERE ci.CartId = @CartId";

                var lookup = new Dictionary<int, CartItem>();

                /* await connection.QueryAsync<CartItem, Product, CartItem>(
                     sql,
                     (item, product) =>
                     {
                         item.Product = product;
                         lookup[item.Id] = item;
                         return item;
                     },
                     new { CartId = cartId },
                     splitOn: "Id"
                 );*/
                await connection.QueryAsync<CartItem, Product, CartItem>(
     sql,
     (item, product) =>
     {
         item.Product = product;
         lookup[item.Id] = item;
         return item;
     },
     new { CartId = cartId },
     splitOn: "ProductIdSplit"
 );


                Logger.LogMessage($"Fetched {lookup.Count} items for cart {cartId}");

                return lookup.Values;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task AddItemAsync(CartItem item)
        {
            Logger.LogMessage($"Adding Product {item.ProductId} to Cart {item.CartId}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    @"INSERT INTO CartItems (CartId, ProductId, Quantity)
                      VALUES (@CartId, @ProductId, @Quantity)",
                    item);

                Logger.LogMessage($"Item added to Cart {item.CartId}");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task UpdateItemAsync(CartItem item)
        {
            Logger.LogMessage($"Updating CartItem {item.Id}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    @"UPDATE CartItems 
                      SET Quantity=@Quantity 
                      WHERE Id=@Id",
                    item);

                Logger.LogMessage($"CartItem {item.Id} updated");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task RemoveItemAsync(int id)
        {
            Logger.LogMessage($"Removing CartItem {id}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    "DELETE FROM CartItems WHERE Id=@Id",
                    new { Id = id });

                Logger.LogMessage($"CartItem {id} removed");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            Logger.LogMessage($"Clearing Cart {cartId}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    "DELETE FROM CartItems WHERE CartId=@CartId",
                    new { CartId = cartId });

                Logger.LogMessage($"Cart {cartId} cleared");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
    }
}
