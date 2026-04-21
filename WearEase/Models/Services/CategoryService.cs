/*using System;
using System.Collections.Generic;
using WearEase.Models.Interfaces;
namespace WearEase.Models.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public List<Category> GetAllCategories()
        {
            try
            {
                return _categoryRepo.GetAll();
            }
            catch (Exception)
            {
                return new List<Category>();
            }
        }

        public Category GetCategory(int id)
        {
            try
            {
                return _categoryRepo.GetById(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddCategory(Category category)
        {
            try
            {
                _categoryRepo.Add(category);
            }
            catch (Exception)
            {
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                _categoryRepo.Update(category);
            }
            catch (Exception)
            {
            }
        }

        public void DeleteCategory(int id)
        {
            try
            {
                _categoryRepo.Delete(id);
            }
            catch (Exception)
            {
            }
        }
    }
}
*/
/*using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _categoryRepo.GetAllAsync();

        public async Task<Category?> GetCategoryAsync(int id)
            => await _categoryRepo.GetByIdAsync(id);

        public async Task AddCategoryAsync(Category category)
            => await _categoryRepo.AddAsync(category);

        public async Task UpdateCategoryAsync(Category category)
            => await _categoryRepo.UpdateAsync(category);

        public async Task DeleteCategoryAsync(int id)
            => await _categoryRepo.DeleteAsync(id);
    }
}
*/
/*using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _categoryRepo.GetAllAsync();

        public async Task<Category?> GetCategoryAsync(int id)
            => await _categoryRepo.GetByIdAsync(id);

        public async Task AddCategoryAsync(Category category)
            => await _categoryRepo.AddAsync(category);

        public async Task UpdateCategoryAsync(Category category)
            => await _categoryRepo.UpdateAsync(category);

        public async Task DeleteCategoryAsync(int id)
            => await _categoryRepo.DeleteAsync(id);
    }
}
*/
/*using WearEase.Models;
using WearEase.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace WearEase.Models.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepo, ILogger<CategoryService> logger)
        {
            _categoryRepo = categoryRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories...");
            var result = await _categoryRepo.GetAllAsync();
            _logger.LogInformation("Fetched {Count} categories.", result.Count());
            return result;
        }

        public async Task<Category?> GetCategoryAsync(int id)
        {
            _logger.LogInformation("Fetching category with ID: {Id}", id);
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null)
                _logger.LogWarning("Category ID {Id} not found.", id);
            else
                _logger.LogInformation("Category found: {Name}", category.Name);

            return category;
        }

        public async Task AddCategoryAsync(Category category)
        {
            _logger.LogInformation("Adding new category: {Name}", category.Name);
            await _categoryRepo.AddAsync(category);
            _logger.LogInformation("Category added successfully.");
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _logger.LogInformation("Updating category ID {Id} with new name: {Name}",
                category.Id, category.Name);

            await _categoryRepo.UpdateAsync(category);

            _logger.LogInformation("Category updated successfully.");
        }

        public async Task DeleteCategoryAsync(int id)
        {
            _logger.LogWarning("Deleting category ID {Id}", id);
            await _categoryRepo.DeleteAsync(id);
            _logger.LogInformation("Category deleted successfully.");
        }
    }
}
*/
using WearEase.Models;
using WearEase.Models.Interfaces;

namespace WearEase.Models.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                Logger.LogMessage("Fetching all categories...");
                var result = await _categoryRepo.GetAllAsync();
                Logger.LogMessage($"Fetched {result.Count()} categories.");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<Category?> GetCategoryAsync(int id)
        {
            try
            {
                Logger.LogMessage($"Fetching category with ID: {id}");
                var category = await _categoryRepo.GetByIdAsync(id);

                if (category == null)
                {
                    Logger.LogMessage($"Category ID {id} not found.");
                }
                else
                {
                    Logger.LogMessage($"Category found: {category.Name}");
                }

                return category;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            try
            {
                Logger.LogMessage($"Adding new category: {category.Name}");
                await _categoryRepo.AddAsync(category);
                Logger.LogMessage("Category added successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                Logger.LogMessage($"Updating category ID {category.Id} with name {category.Name}");
                await _categoryRepo.UpdateAsync(category);
                Logger.LogMessage("Category updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                Logger.LogMessage($"Deleting category ID {id}");
                await _categoryRepo.DeleteAsync(id);
                Logger.LogMessage("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
    }
}
