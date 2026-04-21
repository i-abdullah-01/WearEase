

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WearEase.Models;
using WearEase.Models.Interfaces;
using WearEase.Models.Services; // ✅ your Logger namespace

namespace WearEase.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            Logger.LogMessage("CategoryRepository.GetAllAsync started.");

            try
            {
                using var connection = CreateConnection();
                var categories = await connection.QueryAsync<Category>(
                    "SELECT * FROM Categories");

                Logger.LogMessage($"Fetched {categories.Count()} categories successfully.");
                return categories;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            Logger.LogMessage($"CategoryRepository.GetByIdAsync called with Id={id}");

            try
            {
                using var connection = CreateConnection();

                var category = await connection.QueryFirstOrDefaultAsync<Category>(
                    "SELECT * FROM Categories WHERE Id=@Id",
                    new { Id = id });

                if (category == null)
                {
                    Logger.LogMessage($"Category not found. Id={id}");
                }
                else
                {
                    Logger.LogMessage($"Category retrieved successfully. Id={id}");
                }

                return category;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task AddAsync(Category category)
        {
            Logger.LogMessage($"Adding category. Name={category.Name}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    "INSERT INTO Categories (Name) VALUES (@Name)",
                    category);

                Logger.LogMessage($"Category added successfully. Name={category.Name}");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task UpdateAsync(Category category)
        {
            Logger.LogMessage($"Updating category. Id={category.Id}, Name={category.Name}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    "UPDATE Categories SET Name=@Name WHERE Id=@Id",
                    category);

                Logger.LogMessage($"Category updated successfully. Id={category.Id}");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            Logger.LogMessage($"Deleting category. Id={id}");

            try
            {
                using var connection = CreateConnection();

                await connection.ExecuteAsync(
                    "DELETE FROM Categories WHERE Id=@Id",
                    new { Id = id });

                Logger.LogMessage($"Category deleted successfully. Id={id}");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
    }
}
