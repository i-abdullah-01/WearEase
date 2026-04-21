using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                Logger.LogMessage($"Cart Index accessed by user: {User?.Identity?.Name ?? "Anonymous"}");

                var cart = await _cartService.GetCartAsync();

                if (cart == null)
                {
                    Logger.LogMessage("No cart found for current user.");
                }
                else
                {
                    Logger.LogMessage($"Cart loaded successfully. CartId={cart.Id}, Items={cart.CartItems?.Count ?? 0}");
                }

                return View(cart);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                TempData["Error"] = "Unable to load your cart.";
                return RedirectToAction("Index", "WearEase");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int qty = 1)
        {
            try
            {
                Logger.LogMessage($"Add to cart requested. ProductId={productId}, Qty={qty}");

                await _cartService.AddToCartAsync(productId, qty);

                Logger.LogMessage($"Product added to cart successfully. ProductId={productId}");
                TempData["Success"] = "Product added to cart.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                TempData["Error"] = "Could not add product to cart.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                Logger.LogMessage($"Remove cart item requested. CartItemId={id}");

                await _cartService.RemoveItemAsync(id);

                Logger.LogMessage($"Cart item removed successfully. CartItemId={id}");
                TempData["Success"] = "Item removed from cart.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                TempData["Error"] = "Could not remove item from cart.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                Logger.LogMessage($"Clear cart requested by user: {User?.Identity?.Name ?? "Anonymous"}");

                await _cartService.ClearCartAsync();

                Logger.LogMessage("Cart cleared successfully.");
                TempData["Success"] = "Cart cleared.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                TempData["Error"] = "Could not clear cart.";
                return RedirectToAction("Index");
            }
        }
    }
}

