using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context
                .Orders.Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context
                .Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Preparing,
                Address = "Adresinizi giriniz",
                Items = cart
                    .Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Product?.Price ?? 0,
                    })
                    .ToList(),
                Total = cart.Items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity),
            };
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnOrder(int orderId, string reason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.UserId != userId)
                return NotFound();

            var existingReturn = await _context.ReturnRequests.FirstOrDefaultAsync(r =>
                r.OrderId == orderId
            );
            if (existingReturn != null)
            {
                TempData["ReturnMessage"] = "Bu sipariş için zaten iade talebi gönderilmiş.";
                return RedirectToAction("Index");
            }

            var returnRequest = new ReturnRequest
            {
                OrderId = orderId,
                Reason = reason,
                RequestDate = DateTime.Now,
                IsApproved = false,
            };

            _context.ReturnRequests.Add(returnRequest);

            order.Status = OrderStatus.ReturnRequested;
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                _context.UserMessages.Add(
                    new UserMessage
                    {
                        UserId = admin.Id,
                        Message = $"Yeni bir iade talebi geldi. Sipariş No: {orderId}",
                    }
                );
            }

            await _context.SaveChangesAsync();

            TempData["ReturnMessage"] = "İade talebiniz başarıyla gönderildi.";

            return RedirectToAction("Index");
        }
    }
}
