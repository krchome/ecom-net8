using ECommerceShopingCartASPNET8.Data;
using ECommerceShopingCartASPNET8.Models;
using ECommerceShopingCartASPNET8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceShopingCartASPNET8.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly ProductContext _context;

        public CartController(CartService cartService, ProductContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current cart for the logged-in user
            var cart = await _cartService.GetCartAsync(User.Identity.Name);
            if (cart == null)
            {
                // If no cart exists, create a new one
                cart = new Cart { UserId = User.Identity.Name };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart); // Return the cart view with the cart details
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the current cart for the user
                var cart = await _cartService.GetCartAsync(User.Identity.Name);
                if (cart == null)
                {
                    cart = new Cart { UserId = User.Identity.Name };
                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }

                // Check if the product is already in the cart
                var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == model.ProductId);

                if (existingItem != null)
                {
                    // If product exists, update quantity
                    existingItem.Quantity = model.Quantity;
                    _context.CartItems.Update(existingItem); // Update the existing item in the database
                    await _context.SaveChangesAsync();

                    return Json(new { updated = true });
                }

                // If the product is not in the cart, add it
                var newItem = new Item
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    CartId = cart.Id
                };
                cart.Items.Add(newItem);
                await _context.SaveChangesAsync();

                return Json(new { updated = false });
            }

            return BadRequest(); // Return a BadRequest if model validation fails
        }

        public async Task<IActionResult> Remove(string productid)
        {
            // Fetch the current cart for the user
            var cart = await _cartService.GetCartAsync(User.Identity.Name);
            if (cart == null) return NotFound(); // Return NotFound if the cart does not exist

            // Find the item to remove by productId
            var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == productid);
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove); // Remove the item from the cart
                _context.CartItems.Remove(itemToRemove); // Remove the item from the database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cart"); // Redirect back to the cart index page
        }
    }
}




















//using ECommerceShopingCartASPNET8.Data;
//using ECommerceShopingCartASPNET8.Models;
//using ECommerceShopingCartASPNET8.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace ECommerceShopingCartASPNET8.Controllers
//{
//    public class CartController : Controller
//    {
//        private readonly CartService _cartService;
//        private readonly ProductContext _context;

//        public CartController(CartService cartService, ProductContext context)
//        {
//            _cartService = cartService;
//            _context = context;

//        }
//        public IActionResult Index()
//        {
//            var cart = _cartService.GetCart();
//            return View(cart);
//        }
//        [HttpPost]
//        public IActionResult AddToCart([FromBody] AddToCartViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var cart = _cartService.GetCart();

//                // Check if the product already exists in the cart
//                var existingItem = cart.Items.FirstOrDefault(item => item.Product.Id == model.ProductId);
//                if (existingItem != null)
//                {
//                    // Update the quantity of the existing item
//                    existingItem.Quantity = model.Quantity;
//                    _cartService.UpdateCart(cart);

//                    // Return a JSON response with the updated flag
//                    return Json(new { updated = true });
//                }

//                // The product is not in the cart, add it
//                _cartService.AddToCart(model.ProductId, model.Quantity);
//                _cartService.UpdateCart(cart);

//                // Return a JSON response with the updated flag set to false
//                return Json(new { updated = false });
//            }

//            // Model validation failed, return a BadRequest response
//            return BadRequest();
//        }
//        public IActionResult Remove(string productid)
//        {
//            _cartService.RemoveItem(productid);
//            return RedirectToAction("Index", "Cart");
//        }


//    }
//}
