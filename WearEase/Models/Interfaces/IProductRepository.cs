/*using WearEase.Models;
using System.Collections.Generic;
namespace WearEase.Models.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product GetById(int id);

        void Add(Product product);
        void Update(Product product);
        void Delete(int id);

        List<Product> GetByCategory(int categoryId);
    }
}
*/
using WearEase.Models;

namespace WearEase.Models.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> GetTopFourAsync();
    }
}
