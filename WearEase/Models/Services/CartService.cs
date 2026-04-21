using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IHttpContextAccessor http,
            ILogger<CartService> logger)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _http = http;
            _logger = logger;
        }

        
        private string GetOrCreateCustomerId()
        {
            var context = _http.HttpContext;

            // Logged-in user → stable UserId
            var userId = context?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }

            // Guest user
            if (context!.Request.Cookies.TryGetValue("GuestId", out var guestId))
            {
                return guestId;
            }

            // New guest
            guestId = Guid.NewGuid().ToString();

            context.Response.Cookies.Append(
                "GuestId",
                guestId,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                });

            return guestId;
        }


        public async Task<Cart?> GetCartAsync()
        {
            try
            {
                var customerId = GetOrCreateCustomerId();
                _logger.LogInformation("Fetching cart for customer: {CustomerId}", customerId);

                return await _cartRepo.GetCartByCustomerAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve cart.");
                throw;
            }
        }

        public async Task AddToCartAsync(int productId, int qty)
        {
            try
            {
                _logger.LogInformation("Adding product {ProductId} (Qty: {Qty}) to cart...", productId, qty);

                var customerId = GetOrCreateCustomerId();
                var cart = await _cartRepo.GetCartByCustomerAsync(customerId);

                if (cart == null)
                {
                    _logger.LogInformation("No cart exists for user. Creating new cart for {CustomerId}", customerId);
                    cart = new Cart { CustomerId = customerId };
                    await _cartRepo.CreateCartAsync(cart);
                }

                var product = await _productRepo.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Attempted to add non-existing product ID {ProductId}", productId);
                    return;
                }

                var existing = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

                if (existing == null)
                {
                    _logger.LogInformation("Adding product {ProductId} as new cart item", productId);
                    await _cartRepo.AddItemAsync(new CartItem { CartId = cart.Id, ProductId = productId, Quantity = qty });
                }
                else
                {
                    existing.Quantity += qty;
                    _logger.LogInformation("Updating quantity for product {ProductId} to {Quantity}", productId, existing.Quantity);
                    await _cartRepo.UpdateItemAsync(existing);
                }

                var updatedCount = (await _cartRepo.GetCartItemsAsync(cart.Id)).Sum(i => i.Quantity);
                _http.HttpContext.Session.SetInt32("CartCount", updatedCount);

                _logger.LogInformation("Cart updated successfully. New item count: {Count}", updatedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product {ProductId} to cart.", productId);
                throw;
            }
        }

        public async Task RemoveItemAsync(int id)
        {
            try
            {
                _logger.LogInformation("Removing cart item ID: {Id}", id);
                await _cartRepo.RemoveItemAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cart item {Id}", id);
                throw;
            }
        }

        public async Task ClearCartAsync()
        {
            try
            {
                var cart = await GetCartAsync();
                if (cart == null)
                {
                    _logger.LogWarning("Attempted to clear a null cart");
                    return;
                }

                _logger.LogInformation("Clearing cart ID {CartId}", cart.Id);

                await _cartRepo.ClearCartAsync(cart.Id);
                _http.HttpContext.Session.SetInt32("CartCount", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cart.");
                throw;
            }
        }
    }
}
