using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearEase.Models;
using WearEase.Models.Services;
using WearEase.Models.ViewModels;
using static NuGet.Packaging.PackagingConstants;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly ProductService _productService;
    private readonly CategoryService _categoryService;
    private readonly OrderService _orderService;

    public AdminController(
        ProductService productService,
        CategoryService categoryService,
        OrderService orderService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _orderService = orderService;
    }
    [Authorize(Policy = "ProductManager")]
    public async Task<IActionResult> Products()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    // ---------------- DASHBOARD ----------------
    public async Task<IActionResult> Index()
    {
        Logger.LogMessage("Admin dashboard accessed.");

        try
        {
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();
            var ordersperday=await _orderService.GetOrdersPerDayAsync();

            var vm = new AdminDashboardViewModel
            {

                Products = (await _productService.GetAllProductsAsync()).ToList(),
                Categories = (await _categoryService.GetAllCategoriesAsync()).ToList(),
               // Orders = (await _orderService.GetAllOrdersAsync()).ToList(),
                Orders = orders,

                // ANALYTICS
                TotalOrders = orders.Count,
                TotalRevenue = orders
            .Where(o => o.Status == "Approved" || o.Status == "Delivered")
            .Sum(o => o.TotalAmount),

                ApprovedOrders = orders.Count(o => o.Status == "Approved"),
                PendingOrders = orders.Count(o => o.Status == "Pending"),
                DeliveredOrders = orders.Count(o => o.Status == "Delivered")
                


            };

            Logger.LogMessage("Dashboard data loaded successfully.");
            return View(vm);
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Error loading dashboard.";
            return View("Error");
        }
    }
    public async Task<IActionResult> Index1()
    {
        var ordersperday = await _orderService.GetOrdersPerDayAsync();
        var vm = new AdminDashboardViewModel
        {
            OrderDates = ordersperday
            .Select(o => o.Date.ToString("dd MMM"))
            .ToList(),

            OrdersPerDay = ordersperday
             .Select(o => o.Count)
            .ToList()
        };
         return View(vm);

    }

    // ---------------- CATEGORY ----------------
    [HttpPost]
    [Authorize(Policy = "CategoryManager")]
    public async Task<IActionResult> AddCategory(Category model)
    {
        Logger.LogMessage("AddCategory requested.");

        if (model == null || string.IsNullOrWhiteSpace(model.Name))
        {
            Logger.LogMessage("AddCategory failed: invalid category data.");
            TempData["Error"] = "Category name is required.";
            return RedirectToAction("Index");
        }

        try
        {
            await _categoryService.AddCategoryAsync(model);
            Logger.LogMessage($"Category added successfully. Name: {model.Name}");
            TempData["Success"] = "Category added.";
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to add category.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Policy = "CategoryManager")]
    public async Task<IActionResult> UpdateCategory(Category model)
    {
        Logger.LogMessage($"UpdateCategory requested. ID: {model.Id}");

        try
        {
            await _categoryService.UpdateCategoryAsync(model);
            Logger.LogMessage($"Category updated successfully. ID: {model.Id}");
            TempData["Success"] = "Category updated.";
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to update category.";
        }

        return RedirectToAction("Index");
    }

    [Authorize(Policy = "CategoryManager")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        Logger.LogMessage($"DeleteCategory requested. ID: {id}");

        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            Logger.LogMessage($"Category deleted successfully. ID: {id}");
            TempData["Success"] = "Category deleted.";
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to delete category.";
        }

        return RedirectToAction("Index");
    }

    // ---------------- PRODUCT ----------------
    [HttpPost]
    [Authorize(Policy = "ProductManager")]
    /* public async Task<IActionResult> AddProduct(Product model)
     {
         Logger.LogMessage("AddProduct requested.");

         if (model == null || string.IsNullOrWhiteSpace(model.Name))
         {
             Logger.LogMessage("AddProduct failed: invalid product data.");
             TempData["Error"] = "Enter product details.";
             return RedirectToAction("Index");
         }

         try
         {
             await _productService.AddProductAsync(model);
             Logger.LogMessage($"Product added successfully. Name: {model.Name}");
             TempData["Success"] = "Product added.";
         }
         catch (Exception ex)
         {
             Logger.LogExpception(ex);
             TempData["Error"] = "Failed to add product.";
         }

         return RedirectToAction("Index");
     }*/
   
    public async Task<IActionResult> AddProduct(Product model, IFormFile ImageFile)
    {
        Logger.LogMessage("AddProduct requested.");

        if (model == null || string.IsNullOrWhiteSpace(model.Name) || ImageFile == null)
        {
            Logger.LogMessage("AddProduct failed: invalid data.");
            TempData["Error"] = "Product name and image are required.";
            return RedirectToAction("Index");
        }

        try
        {
            // ---------------- SAVE IMAGE ----------------
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            // Save relative path in DB
            model.ImageUrl = "/images/products/" + uniqueFileName;

            // ---------------- SAVE PRODUCT ----------------
            await _productService.AddProductAsync(model);

            Logger.LogMessage($"Product added successfully. Name: {model.Name}");
            TempData["Success"] = "Product added successfully.";
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to add product.";
        }

        return RedirectToAction("Index");
    }
    [Authorize(Policy = "ProductManager")]
    public async Task<IActionResult> EditProduct(int id)
    {
        Logger.LogMessage($"EditProduct page opened. ID: {id}");

        var product = await _productService.GetProductAsync(id);

        if (product == null)
        {
            TempData["Error"] = "Product not found.";
            return RedirectToAction("Index");
        }

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View(product);
    }


    /*[HttpPost]
    [Authorize(Policy = "ProductManager")]
    
    public async Task<IActionResult> UpdateProduct(Product model, IFormFile? ImageFile)
    {
        try
        {
            if (ImageFile != null)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream);

                model.ImageUrl = "/images/" + fileName;
            }

            await _productService.UpdateProductAsync(model);
            Logger.LogMessage($"Product updated successfully. ID: {model.Id}");
            TempData["Success"] = "Product updated.";
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to update product.";
        }

        return RedirectToAction("Index");
    }*/
    /* [HttpPost]
     [Authorize(Policy = "ProductManager")]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> UpdateProduct(Product model, IFormFile? ImageFile)
     {
         try
         {
             var existingProduct = await _productService.GetProductAsync(model.Id);
             if (existingProduct == null)
             {
                 TempData["Error"] = "Product not found.";
                 return RedirectToAction("Products");
             }

             // Update scalar fields
             existingProduct.Name = model.Name;
             existingProduct.Price = model.Price;
             existingProduct.Description = model.Description;
             existingProduct.CategoryId = model.CategoryId;

             // Image handling
             if (ImageFile != null)
             {
                 string uploadsFolder = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot",
                     "images",
                     "products"
                 );

                 if (!Directory.Exists(uploadsFolder))
                     Directory.CreateDirectory(uploadsFolder);

                 string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                 string filePath = Path.Combine(uploadsFolder, fileName);

                 using var stream = new FileStream(filePath, FileMode.Create);
                 await ImageFile.CopyToAsync(stream);

                 existingProduct.ImageUrl = "/images/products/" + fileName;
             }

             await _productService.UpdateProductAsync(existingProduct);

             TempData["Success"] = "Product updated successfully.";
             return RedirectToAction("Products");
         }
         catch (Exception ex)
         {
             Logger.LogExpception(ex);
             TempData["Error"] = "Failed to update product.";
             return RedirectToAction("Products");
         }
     }
 */
    [HttpPost]
    [Authorize(Policy = "ProductManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProduct(Product model, IFormFile? ImageFile)
    {
        try
        {
            var existingProduct = await _productService.GetProductAsync(model.Id);
            if (existingProduct == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Products");
            }

            existingProduct.Name = model.Name;
            existingProduct.Price = model.Price;
            existingProduct.Description = model.Description;
            existingProduct.CategoryId = model.CategoryId;

            if (ImageFile != null)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "products"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream);

                existingProduct.ImageUrl = "/images/products/" + fileName;
            }

            await _productService.UpdateProductAsync(existingProduct);

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to update product.";
            return RedirectToAction("Index");
        }
    }


    [Authorize(Policy = "ProductManager")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        Logger.LogMessage($"DeleteProduct requested. ID: {id}");

        try
        {
            await _productService.DeleteProductAsync(id);
            Logger.LogMessage($"Product deleted successfully. ID: {id}");
            TempData["Success"] = "Product deleted.";
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to delete product.";
        }

        return RedirectToAction("Index");
    }

    // ---------------- ORDERS ----------------
    [Authorize(Policy = "OrderManager")]
    public async Task<IActionResult> OrderDetails(int id)
    {
        Logger.LogMessage($"OrderDetails requested. OrderId: {id}");

        try
        {
            var order = await _orderService.GetOrderAsync(id);

            if (order == null)
            {
                Logger.LogMessage($"Order not found. OrderId: {id}");
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            return View(order);
        }
        catch (Exception ex)
        {
            Logger.LogExpception(ex);
            TempData["Error"] = "Failed to load order details.";
            return RedirectToAction("Index");
        }
    }
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        await _orderService.ApproveOrderAsync(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Deliver(int id)
    {
        await _orderService.MarkDeliveredAsync(id);
        return RedirectToAction("Index");
    }
   
    


}
