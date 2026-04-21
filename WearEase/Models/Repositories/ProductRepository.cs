using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WearEase.Models;
using WearEase.Models.Interfaces;
using WearEase.Models.Services;   // <-- IMPORTANT: Logger namespace

namespace WearEase.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            const string sql = @"
                SELECT p.*, c.Id, c.Name
                FROM Products p
                INNER JOIN Categories c ON p.CategoryId = c.Id";

            try
            {
                Logger.LogMessage("Fetching all products.");

                using var connection = CreateConnection();

                var result = await connection.QueryAsync<Product, Category, Product>(
                    sql,
                    (product, category) =>
                    {
                        product.Category = category;
                        return product;
                    },
                    splitOn: "Id"
                );

                Logger.LogMessage($"Fetched {result.Count()} products successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetTopFourAsync()
        {
            const string sql = @"
        SELECT TOP 4 
            p.*, 
            c.Id, 
            c.Name
        FROM Products p
        INNER JOIN Categories c ON p.CategoryId = c.Id
        ORDER BY p.Id DESC";

    try
            {
                Logger.LogMessage("Fetching top 4 latest products.");

                using var connection = CreateConnection();

                var result = await connection.QueryAsync<Product, Category, Product>(
                    sql,
                    (product, category) =>
                    {
                        product.Category = category;
                        return product;
                    },
                    splitOn: "Id"
                );

                Logger.LogMessage($"Fetched {result.Count()} products successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT p.*, c.Id, c.Name
                FROM Products p
                INNER JOIN Categories c ON p.CategoryId = c.Id
                WHERE p.Id = @Id";

            try
            {
                Logger.LogMessage($"Fetching product with ID {id}.");

                using var connection = CreateConnection();

                var result = await connection.QueryAsync<Product, Category, Product>(
                    sql,
                    (product, category) =>
                    {
                        product.Category = category;
                        return product;
                    },
                    new { Id = id },
                    splitOn: "Id"
                );

                var product = result.FirstOrDefault();

                if (product == null)
                    Logger.LogMessage($"Product with ID {id} not found.");
                else
                    Logger.LogMessage($"Product with ID {id} fetched successfully.");

                return product;
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        /* public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
         {
             const string sql = "SELECT * FROM Products WHERE CategoryId=@CategoryId";

             try
             {
                 Logger.LogMessage($"Fetching products for CategoryId {categoryId}.");

                 using var connection = CreateConnection();
                 var products = await connection.QueryAsync<Product>(sql, new { CategoryId = categoryId });

                 Logger.LogMessage($"Fetched {products.Count()} products for CategoryId {categoryId}.");
                 return products;
             }
             catch (Exception ex)
             {
                 Logger.LogExpception(ex);
                 throw;
             }
         }*/
        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            const string sql = @"
        SELECT p.*, c.Id, c.Name
        FROM Products p
        INNER JOIN Categories c ON p.CategoryId = c.Id
        WHERE p.CategoryId = @CategoryId";

            using var connection = CreateConnection();

            return await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { CategoryId = categoryId },
                splitOn: "Id"
            );
        }

        public async Task AddAsync(Product product)
        {
            const string sql = @"
                INSERT INTO Products
                (Name, Price, Description, ImageUrl, CategoryId, TotalQuantity, RemainingQuantity)
                VALUES
                (@Name, @Price, @Description, @ImageUrl, @CategoryId, @TotalQuantity, @RemainingQuantity)";

            try
            {
                Logger.LogMessage($"Adding new product: {product.Name}");

                using var connection = CreateConnection();
                await connection.ExecuteAsync(sql, product);

                Logger.LogMessage($"Product '{product.Name}' added successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            const string sql = @"
                UPDATE Products SET
                Name=@Name,
                Price=@Price,
                Description=@Description,
                ImageUrl=@ImageUrl,
                CategoryId=@CategoryId,
                TotalQuantity=@TotalQuantity,
                RemainingQuantity=@RemainingQuantity
                WHERE Id=@Id";

            try
            {
                Logger.LogMessage($"Updating product ID {product.Id}.");

                using var connection = CreateConnection();
                await connection.ExecuteAsync(sql, product);

                Logger.LogMessage($"Product ID {product.Id} updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Products WHERE Id=@Id";

            try
            {
                Logger.LogMessage($"Deleting product with ID {id}.");

                using var connection = CreateConnection();
                await connection.ExecuteAsync(sql, new { Id = id });

                Logger.LogMessage($"Product ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogExpception(ex);
                throw;
            }
        }
    }
}
