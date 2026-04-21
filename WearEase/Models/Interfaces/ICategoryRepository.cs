/*using WearEase.Models;
using System.Collections.Generic;
namespace WearEase.Models.Interfaces
{
    public interface ICategoryRepository
    {
        //List<Category> GetAll();
        Task<IEnumerable<Category>> GetAllAsync();
        Category GetById(int id);

        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }

}
*/
using WearEase.Models;

namespace WearEase.Models.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);

        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
