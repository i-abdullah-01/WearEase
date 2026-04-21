/*using WearEase.Models;
using System.Collections.Generic;
namespace WearEase.Models.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCartByCustomer(string customerId);

        void CreateCart(Cart cart);

        void AddItem(CartItem item);
        void UpdateItem(CartItem item);
        void RemoveItem(int cartItemId);

        List<CartItem> GetCartItems(int cartId);

        void ClearCart(int cartId);
    }
}
*/
/*using WearEase.Models;

namespace WearEase.Models.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByCustomerAsync(string customerId);
        Task CreateCartAsync(Cart cart);

        Task<List<CartItem>> GetCartItemsAsync(int cartId);

        Task AddItemAsync(CartItem item);
        Task UpdateItemAsync(CartItem item);
        Task RemoveItemAsync(int cartItemId);

        Task ClearCartAsync(int cartId);
    }
}
*/
/*using WearEase.Models;

namespace WearEase.Models.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByCustomerAsync(string customerId);
        Task ClearCartAsync(int cartId);
    }
}*/
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WearEase.Models.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByCustomerAsync(string customerId);

        Task CreateCartAsync(Cart cart);

        Task AddItemAsync(CartItem item);
        Task UpdateItemAsync(CartItem item);
        Task RemoveItemAsync(int id);

        Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);

        Task ClearCartAsync(int cartId);
    }
}

