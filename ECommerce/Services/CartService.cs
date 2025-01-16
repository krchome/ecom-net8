using ECommerceShopingCartASPNET8.Data;
using ECommerceShopingCartASPNET8.Models;
using ECommerceShopingCartASPNET8.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

public class CartService : ICartService
{
    private readonly ProductContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(ProductContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    // Get the current user's cart (asynchronously)
    public async Task<Cart> GetCartAsync(string userId)
    {

        if (string.IsNullOrEmpty(userId))
        {
            //throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
            userId = "Guest";
        }

        // Try to fetch the cart for the user
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            return cart; // Return the existing cart
        }

        // Create a new cart if it doesn't exist
        cart = new Cart
        {
            Id = Guid.NewGuid().ToString(), // Ensure the primary key is set
            UserId = userId ?? "Guest",
            Items = new List<Item>() // Initialize an empty list of items
        };

        await _context.Carts.AddAsync(cart); // Add the new cart to the database
        await _context.SaveChangesAsync();

        return cart;
    }

    // Add a product to the cart
    public async Task AddToCartAsync(string userId, string productId, int quantity)
    {
        var cart = await GetCartAsync(userId);
        var product = await _context.Products.FindAsync(productId);

        if (product != null)
        {
            // Check if the product is already in the cart
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                // If the product exists, update its quantity
                existingItem.Quantity += quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                // If the product doesn't exist in the cart, add a new item
                var newItem = new Item
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.Id
                };
                cart.Items.Add(newItem);
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
        }
    }

    // Update the cart (used for updating an existing cart, if needed)
    public async Task UpdateCartAsync(Cart cart)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
    }

    // Remove an item from the cart
    public async Task RemoveItemAsync(string userId, string productId)
    {
        var cart = await GetCartAsync(userId);
        var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (itemToRemove != null)
        {
            cart.Items.Remove(itemToRemove);
            _context.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();
        }
    }

    // Clear the cart (i.e., remove all items from the cart)
    public async Task ClearCartAsync(string userId)
    {
        var cart = await GetCartAsync(userId);
        if (cart != null)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
