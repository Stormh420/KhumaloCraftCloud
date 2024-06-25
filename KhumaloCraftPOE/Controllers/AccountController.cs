using KhumaloCraftPOE.Data;
using KhumaloCraftPOE.Models;
using KhumaloCraftPOE.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhumaloCraftPOE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Account()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userOrders = await _context.Orders.Where(o => o.UserID == user.UserId).ToListAsync();
            var productsForSale = await _context.Products.Where(p => p.SellerID == user.UserId).ToListAsync();
            var ordersOnMyProducts = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => productsForSale.Select(p => p.ProductID).Contains(oi.ProductID)))
                .ToListAsync();

            var model = new AccountViewModel(user, userOrders, productsForSale, ordersOnMyProducts);
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                product.SellerID = user.UserId;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Account");
            }
            return View(product);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Hash the password before saving it
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Users.Add(user);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Log the exception (use a logging framework or any logging mechanism)
                    ModelState.AddModelError("", "An error occurred while registering the user. Please try again.");
                    Console.WriteLine(ex.Message);
                }
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    var claims = new Claim[] {
                        new Claim(ClaimTypes.Name, user.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Account"); // Corrected action name
                }
                else
                {
                    // Log incorrect password attempt
                    ModelState.AddModelError("", "Invalid password");
                    Console.WriteLine("Invalid password attempt for user: " + email);
                }
            }
            else
            {
                // Log email not found
                ModelState.AddModelError("", "Invalid email");
                Console.WriteLine("Invalid email attempt: " + email);
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}









