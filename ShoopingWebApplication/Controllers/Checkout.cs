using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoopingWebApplication.Data;
using ShoopingWebApplication.Models;
using System.Security.Claims;
using ShoopingWebApplication.Data.Migrations;

namespace ShoopingWebApplication.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CheckoutController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OrderConfirmation()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                // Handle unauthenticated users
                return RedirectToAction("Login", "Account");
            }

            var userCart = _context.Carts
     .Include(cart => cart.CartItems)
         .ThenInclude(cartItem => cartItem.Product) 
     .FirstOrDefault(cart => cart.UserId == currentUserId);


            if (userCart == null || !userCart.CartItems.Any())
            {
                // Handle empty cart
                return RedirectToAction("EmptyCart");
            }

            try
            {
                

                var totalAmount = userCart.CartItems.Sum(item => item.Product.Price * item.Quantity);

                var newOrder = new Order
                {
                    UserId = currentUserId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    OrderStatus="Pending"
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                foreach (var cartItem in userCart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                       
                    };

                    _context.OrderItems.Add(orderItem);
                }

                _context.CartItems.RemoveRange(userCart.CartItems);
                _context.SaveChanges();

                var orderConfirmation = new OrderConfirmation
                {
                    CustomerName = user.UserName
                    
                };

                

                return View("OrderConfirmation", orderConfirmation);
            }
            catch (Exception ex)
            {
                
                return RedirectToAction("Error", "Home"); 
            }
        }
    }
}
