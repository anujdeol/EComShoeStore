using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoopingWebApplication.Data;
using ShoopingWebApplication.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace ShoopingWebApplication.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult OrderHistory()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                // Handle unauthenticated users
                return RedirectToAction("Login", "Account");
            }

            var userOrders = _context.Orders
                .Where(o => o.UserId == currentUserId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(userOrders);
        }

        [HttpPost]
        public IActionResult PlaceOrder()
        {
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userCart = _context.Carts
                .Include(cart => cart.CartItems)
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
                    TotalAmount = totalAmount
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

                return RedirectToAction("OrderConfirmation");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home"); 
            }
        }

        public IActionResult EmptyCart()
        {
            return View();
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

       
    }
}
