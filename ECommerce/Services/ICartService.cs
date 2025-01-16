using ECommerceShopingCartASPNET8.Models;

namespace ECommerceShopingCartASPNET8.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string userId);               // Get cart asynchronously for a specific user
        Task AddToCartAsync(string userId, string productId, int quantity); // Add product to cart asynchronously
        Task UpdateCartAsync(Cart cart);                      // Update cart asynchronously
        Task RemoveItemAsync(string userId, string productId); // Remove item from cart asynchronously
        Task ClearCartAsync(string userId);
    }
}
