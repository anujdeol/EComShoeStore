using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoopingWebApplication.Data;
using ShoopingWebApplication.Models;
using System.Linq;
using System.Security.Claims;

namespace ShoopingWebApplication.Controllers
{
    [Authorize]
    public class AddToCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddToCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var productToAdd = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            if (productToAdd == null || quantity <= 0)
            {
                return RedirectToAction("Index", "Product");
            }

            var userCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == currentUserId);

            if (userCart == null)
            {
                userCart = new Cart { UserId = currentUserId };
                _context.Carts.Add(userCart);
                _context.SaveChanges(); 
            }

            var existingCartItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = userCart.CartId
                };

                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }
    }
}
