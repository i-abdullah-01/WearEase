using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;
using WearEase.Models.ViewModels;

namespace WearEase.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class CheckoutController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        public CheckoutController(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartAsync();

           /* if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }*/

            var subTotal = cart.CartItems.Sum(i => i.Product.Price * i.Quantity);
            var shipping = 5m;

            var vm = new CheckoutViewModel
            {
                Cart = cart,
                SubTotal = subTotal,
                Shipping = shipping,
                Total = subTotal + shipping
            };

            return View(vm);
        }
        /*
                [HttpPost]
                [HttpPost]
                public IActionResult PlaceOrder()
                {
                    return RedirectToAction("PlaceOrder", "Order");
                }
        */
        /* [HttpPost]
         public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
         {
             if (string.IsNullOrEmpty(model.PaymentMethod))
             {
                 TempData["Error"] = "Select a payment method.";
                 return RedirectToAction("Index");
             }

             var userId = User.Identity.Name;

             var orderId = await _orderService.PlaceOrderAsync(
                 userId,
                 model.PaymentMethod
             );

             return RedirectToAction("Confirmation", "Order", new { id = orderId });
         }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            // Card validation only if Card selected
            if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrEmpty(model.CardNumber) ||
                    string.IsNullOrEmpty(model.Expiry) ||
                    string.IsNullOrEmpty(model.CVV))
                {
                    ModelState.AddModelError("", "Please fill card details.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Reload cart again
                model.Cart = await _cartService.GetCartAsync();
                return View("Index", model);
            }

            var userId = User.Identity.Name;

            var orderId = await _orderService.PlaceOrderAsync(
                userId,
                model.PaymentMethod
            );

            return RedirectToAction("Confirmation", "Order", new { id = orderId });
        }

    }
}
