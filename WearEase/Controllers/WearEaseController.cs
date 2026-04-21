using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using WearEase.Models.Interfaces;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    public class WearEaseController : Controller
    {

       
        private readonly ProductService _productService;
        private readonly IProductRepository _productRepo;

        public WearEaseController(ProductService productService , IProductRepository productRepo)
        {
            _productService = productService;
            _productRepo = productRepo;
        }

        public async Task<IActionResult> Index()
        {
            

            // show only 4 featured products
            var featured = await _productRepo.GetTopFourAsync();

            return View(featured);
        }
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
