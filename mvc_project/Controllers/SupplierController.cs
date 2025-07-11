using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.IsAdmin = User.IsInRole("Supplier");
            return View();

            return View();
        }

        // Onay bekleyen siparişler
        public async Task<IActionResult> Pending()
        {
            var pendingOrders = await _context
                .SupplierOrders.Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Status == OrderStatus.Preparing)
                .ToListAsync();

            return View(pendingOrders);
        }

        // Onayla
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context
                .SupplierOrders.Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Confirmed;
            order.ConfirmedDate = DateTime.Now;

            // İsteğe bağlı: stoğa düşebilir
            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Pending");
        }

        // Reddet
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var order = await _context.SupplierOrders.FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return RedirectToAction("Pending");
        }
        
    }
}
