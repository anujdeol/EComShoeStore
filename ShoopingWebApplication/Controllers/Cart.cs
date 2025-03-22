using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoopingWebApplication.Data;
using ShoopingWebApplication.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public IActionResult AddToCart(int productId)
    {
        var productToAdd = _context.Products.FirstOrDefault(p => p.ProductId == productId);
        string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (productToAdd != null && !string.IsNullOrEmpty(currentUserId))
        {
            var userCart = _context.Carts
                .Include(cart => cart.CartItems)
                .FirstOrDefault(cart => cart.UserId == currentUserId);

            if (userCart == null)
            {
                // Create a new cart for the user if it doesn't exist
                userCart = new Cart { UserId = currentUserId };
                _context.Carts.Add(userCart);
            }

            var existingCartItem = userCart.CartItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity++; 
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    CartId = userCart.CartId
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult Index()
    {
        string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(currentUserId))
        {
            var cartProducts = _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == currentUserId)
                .ToList(); 

            return View(cartProducts);
        }

        return RedirectToAction("Login", "Account");
    }
    public IActionResult remove(int productId)
    {
        try
        {
            

            var cartItemToRemove = _context.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId );

            Debug.WriteLine(cartItemToRemove);
            Debug.WriteLine("testing product id "+productId);
            
                _context.CartItems.Remove(cartItemToRemove);
                _context.SaveChanges();
                Console.WriteLine("Product removed successfully.");
            
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing cart item: {ex.Message}");
        }

        return RedirectToAction("Index");
    }

}
