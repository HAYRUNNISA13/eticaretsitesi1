using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    [Authorize(Roles = "WarehouseManager")]
    public class WarehouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context
                .Orders.Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> AllOrders()
        {
            var allOrders = await _context
                .Orders.Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(allOrders);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context
                .SupplierOrders.Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            return View(orders);
        }

        // GET: Create sayfası için ürünleri yükle
        public async Task<IActionResult> Create()
        {
            var products = await _context.Products.ToListAsync();

            var vm = new SupplierOrderCreateViewModel
            {
                Products = products,
                OrderItems = products
                    .Select(p => new SupplierOrderItemInput { ProductId = p.Id })
                    .ToList(),
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplierOrderCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Products = await _context.Products.ToListAsync();
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new SupplierOrder
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Preparing,
                Address = vm.Address,
            };

            foreach (var item in vm.OrderItems.Where(i => i.Quantity > 0))
            {
                // Sipariş ürünlerini ekle
                var orderItem = new SupplierOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = 0,
                };
                order.Items.Add(orderItem);

                // Aynı zamanda beklenen stoklara da ekle
                var incoming = new IncomingStock
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ExpectedDate = DateTime.Now.AddDays(7), // Teslim tarihi tahmini (örnek: 7 gün sonra)
                    SupplierName = "Varsayılan Tedarikçi",
                };
                _context.IncomingStocks.Add(incoming);
            }

            _context.SupplierOrders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Inventory()
        {
            var products = await _context
                .Products.Include(p => p.Campaign)
                .Include(p => p.Category)
                .ToListAsync();

            var incomingStocks = await _context
                .IncomingStocks.Include(s => s.Product)
                .ToListAsync();

            var model = new InventoryViewModel
            {
                Products = products,
                IncomingStocks = incomingStocks,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            if (!Enum.TryParse<OrderStatus>(newStatus, out var statusEnum))
            {
                ModelState.AddModelError("", "Geçersiz sipariş durumu.");
                return RedirectToAction(nameof(Index));
            }

            var order = await _context
                .Orders.Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            var previousStatus = order.Status;

            order.Status = statusEnum;

            if (previousStatus == OrderStatus.Preparing && statusEnum == OrderStatus.Shipped)
            {
                order.ShippedDate = DateTime.Now;

                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                    }
                }
            }
            _context.UserMessages.Add(
                new UserMessage
                {
                    UserId = order.UserId,
                    Message = $"Siparişinizin durumu '{newStatus}' olarak güncellendi.",
                }
            );

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
