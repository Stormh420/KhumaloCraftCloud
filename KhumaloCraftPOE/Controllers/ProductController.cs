using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhumaloCraftPOE.Data;
using KhumaloCraftPOE.Models.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace KhumaloCraftPOE.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDurableOrchestrationClient _durableClient;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, IDurableOrchestrationClient durableClient, ILogger<ProductController> logger)
        {
            _context = context;
            _durableClient = durableClient;
            _logger = logger;
        }

        public async Task<IActionResult> ForSale()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userEmail = GetUserEmail();
            if (userEmail == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == user.UserId);
            if (cart == null)
            {
                cart = new Cart { UserID = user.UserId, TotalAmount = 0 };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = new CartItem { CartID = cart.CartID, ProductID = productId, Quantity = quantity };
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> Cart()
        {
            var userEmail = GetUserEmail();
            if (userEmail == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == user.UserId);
            if (cart == null)
            {
                return View(new List<CartItem>());
            }

            var cartItems = await _context.CartItems
                .Where(ci => ci.CartID == cart.CartID)
                .Include(ci => ci.Product)
                .ToListAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userEmail = GetUserEmail();
            if (userEmail == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                                           .FirstOrDefaultAsync(c => c.UserID == user.UserId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Cart");
            }

            var order = new Order
            {
                UserID = user.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                DeliveryLocation = "Default Location"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                _ = _context.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });
            }

            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Starting order processing for order {order.OrderID}");
            await _durableClient.StartNewAsync("OrderOrchestrator", order.OrderID.ToString()); // Convert int to string

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        private string? GetUserEmail()
        {
            return User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}








