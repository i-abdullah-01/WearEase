
using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                Logger.LogMessage("Fetching all products");
                var products = await _productRepo.GetAllAsync();
                Logger.LogMessage($"Fetched {products.Count()} products");
                return products;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            try
            {
                Logger.LogMessage($"Fetching product ID {id}");
                return await _productRepo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }



        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                Logger.LogMessage($"Fetching products for category ID {categoryId}");
                return await _productRepo.GetByCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            try
            {
                Logger.LogMessage($"Adding product {product.Name}");
                await _productRepo.AddAsync(product);
                Logger.LogMessage("Product added successfully");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                Logger.LogMessage($"Updating product ID {product.Id}");
                await _productRepo.UpdateAsync(product);
                Logger.LogMessage("Product updated successfully");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                Logger.LogMessage($"Deleting product ID {id}");
                await _productRepo.DeleteAsync(id);
                Logger.LogMessage("Product deleted successfully");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
    }
}
