
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(
            OrderService orderService,
            UserManager<IdentityUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                Logger.LogMessage($"Checkout page requested by UserId: {userId}");

                var orders = await _orderService.GetOrdersByCustomerAsync(userId);
                return View(orders);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                return RedirectToAction("Index", "Home");
            }
        }
       [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string paymentMethod)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(paymentMethod))
                {
                    TempData["Error"] = "Please select payment method.";
                    return RedirectToAction("Checkout");
                }

                 var orderId = await _orderService.PlaceOrderAsync(userId, paymentMethod);
               


                if (orderId == 0)
                    return RedirectToAction("Checkout");

                return RedirectToAction("Confirmation", new { id = orderId });
            }
            catch
            {
                return RedirectToAction("Checkout");
            }
        }


        /* [HttpPost]
         public async Task<IActionResult> PlaceOrder(string paymentMethod)
         {
             try
             {
                 var userId = _userManager.GetUserId(User);
                 Logger.LogMessage($"PlaceOrder started by UserId: {userId}");

                 var orderId = await _orderService.PlaceOrderAsync(userId, paymentMethod);

                 if (orderId == 0)
                 {
                     Logger.LogMessage($"PlaceOrder failed – cart empty for UserId: {userId} with Payment: {paymentMethod}");
                     return RedirectToAction("Checkout");
                 }

                 Logger.LogMessage($"Order placed successfully. OrderId: {orderId}");
                 return RedirectToAction("Confirmation", new { id = orderId });
             }
             catch (Exception ex)
             {
                 Logger.LogExpception(ex);
                 return RedirectToAction("Checkout");
             }
         }
 */
        /*public async Task<IActionResult> PlaceOrder(string paymentMethod)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                Logger.LogMessage($"PlaceOrder started by UserId: {userId} with Payment: {paymentMethod}");

                var orderId = await _orderService.PlaceOrderAsync(userId, paymentMethod);

                if (orderId == 0)
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Checkout");
                }

                return RedirectToAction("Confirmation", new { id = orderId });
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                return RedirectToAction("Checkout");
            }
        }
*/

        public async Task<IActionResult> Confirmation(int id)
        {
            try
            {
                Logger.LogMessage($"Order confirmation requested for OrderId: {id}");

                var order = await _orderService.GetOrderAsync(id);
                return View(order);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _orderService.GetOrdersByCustomerAsync(userId);

            return View(orders);
        }

    }
}
