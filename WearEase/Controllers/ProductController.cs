using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models;
using WearEase.Models.Services;

namespace WearEase.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductController(
            ProductService productService,
            CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /* public async Task<IActionResult> Index()
         {
             Logger.LogMessage("Product Index page requested.");

             try
             {
                 ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                 var products = await _productService.GetAllProductsAsync();

                 Logger.LogMessage($"Loaded {products.Count()} products.");
                 return View(products);
             }
             catch (Exception ex)
             {
                 Logger.LogExpception(ex);
                 return View("Error");
             }
         }*/
        /*public async Task<IActionResult> Index(int? categoryId)
         {
             Logger.LogMessage("Product Index page requested.");

             ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();


             IEnumerable<Product> products;

             if (categoryId.HasValue && categoryId > 0)
             {
                 products = await _productService.GetByCategoryAsync(categoryId.Value);
             }
             else
             {
                 products = await _productService.GetAllProductsAsync();
             }

             ViewBag.SelectedCategoryId = categoryId;
             ViewBag.SelectedCategoryName = "All Products";

             if (categoryId.HasValue)
             {
                 var category = (await _categoryService.GetAllCategoriesAsync())
                                .FirstOrDefault(c => c.Id == categoryId);

                 if (category != null){
                     ViewBag.SelectedCategoryName = category.Name + " Collection";
                 return View(products);
                      }
             }

           //  return View(products);


             return View(products);
         }*/

        private int GetCategoryOrder(string categoryName)
        {
            categoryName = categoryName.Trim().ToLower();

            return categoryName switch
            {
                "men" => 1,
                "woman" => 2,
                "kids" => 3,
                _ => 999 // newly added categories always last
            };
        }


        public async Task<IActionResult> Index(int? categoryId)
           {
               Logger.LogMessage("Product Index page requested.");

               // ✅ Strongly typed categories
               var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();

               ViewBag.Categories = categories;

               List<Product> products;

               if (categoryId.HasValue && categoryId > 0)
               {
                     products = (await _productService
                            .GetByCategoryAsync(categoryId.Value))
                            .OrderByDescending(p => p.Id) // NEWEST FIRST
                            .ToList();


                // ✅ LINQ on strongly typed list
                var category = categories.FirstOrDefault(c => c.Id == categoryId);

                   ViewBag.SelectedCategoryName = category != null
                       ? category.Name + " Collection"
                       : "Products";
               }
               else
               {
                   products = (await _productService
                               .GetAllProductsAsync())
                               .ToList();

                   products = products
                      // .OrderBy(p => p.Category.Name)
                      .OrderBy(p => GetCategoryOrder(p.Category.Name)) // FIXED CATEGORY ORDER
                       .ThenByDescending(p => p.Id)
                       .ToList();

                   ViewBag.SelectedCategoryName = "All Products";
               }

               ViewBag.SelectedCategoryId = categoryId;

               return View(products);
           }

        public async Task<IActionResult> Details(int id)
        {
            Logger.LogMessage($"Product details requested for ProductId: {id}");

            try
            {
                var product = await _productService.GetProductAsync(id);

                if (product == null)
                {
                    Logger.LogMessage($"Product not found. ProductId: {id}");
                    return RedirectToAction("Index");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                ViewBag.SearchTerm = term;
                ViewBag.Message = "Please enter at least 2 characters to search.";
                return View("SearchResults", new List<Product>());
            }

            var products = await _productService.GetAllProductsAsync();

            var searchResults = products
                .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Id)
                .ToList();

            ViewBag.SearchTerm = term;
            ViewBag.ResultCount = searchResults.Count;

            // Return JSON for AJAX dropdown
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(searchResults.Take(8).Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl
                }));
            }

            // Return view for full page results
            return View("SearchResults", searchResults);
        }

    }
}
