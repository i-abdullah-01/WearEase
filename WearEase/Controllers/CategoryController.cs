/*using Microsoft.AspNetCore.Mvc;
using WearEase.Models;
using WearEase.Models.Interfaces;
using WearEase.Models.Services;
namespace WearEase.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public CategoryController(CategoryService categoryService, ProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // GET: /Category
        public IActionResult Index()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
                return View(categories);
            }
            catch
            {
                return View(new List<Category>());
            }
        }

        // GET: /Category/Products/5
        public IActionResult Products(int id)
        {
            try
            {
                var category = _categoryService.GetCategory(id);
                if (category == null)
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                var products = _productService.GetProductsByCategory(id);

                ViewBag.CategoryName = category.Name;

                return View(products);
            }
            catch
            {
                TempData["Error"] = "Unable to load products.";
                return RedirectToAction("Index");
            }
        }
    }
}

*/
/*using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public CategoryController(CategoryService categoryService, ProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: /Category/Products/5
        public async Task<IActionResult> Products(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Index");
            }

            var products = await _productService.GetByCategoryAsync(id);
            ViewBag.CategoryName = category.Name;

            return View(products);
        }
    }
}*/

/*using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            CategoryService categoryService,
            ProductService productService,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _logger = logger;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Category Index page requested.");

            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                _logger.LogInformation("Fetched {Count} categories.", categories.Count());

                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading categories.");
                return View(new List<object>());
            }
        }

        // GET: /Category/Products/5
        public async Task<IActionResult> Products(int id)
        {
            _logger.LogInformation("Products requested for CategoryId: {CategoryId}", id);

            try
            {
                var category = await _categoryService.GetCategoryAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category not found with Id: {CategoryId}", id);
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                var products = await _productService.GetByCategoryAsync(id);

                _logger.LogInformation(
                    "Loaded {ProductCount} products for CategoryId: {CategoryId}",
                    products.Count(), id
                );

                ViewBag.CategoryName = category.Name;
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products for CategoryId: {CategoryId}", id);
                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("Index");
            }
        }
    }
}
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public CategoryController(
            CategoryService categoryService,
            ProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var result = categories.Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    productCount = c.Products?.Count ?? 0
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                return StatusCode(500, "Error loading categories");
            }
        }

        public async Task<IActionResult> Index()
        {
            Logger.LogMessage("Category Index page accessed.");

            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                Logger.LogMessage($"Loaded {categories.Count()} categories.");
                return View(categories);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                return View(new List<object>());
            }
        }

        public async Task<IActionResult> Products(int id)
        {
            Logger.LogMessage($"Products requested for CategoryId={id}");

            try
            {
                var category = await _categoryService.GetCategoryAsync(id);

                if (category == null)
                {
                    Logger.LogMessage($"Category not found. CategoryId={id}");
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                var products = await _productService.GetByCategoryAsync(id);

                Logger.LogMessage(
                    $"Loaded {products.Count()} products for CategoryId={id}"
                );

                ViewBag.CategoryName = category.Name;
                return View(products);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("Index");
            }
        }
    }
}
