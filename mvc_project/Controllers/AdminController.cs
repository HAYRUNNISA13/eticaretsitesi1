using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View();
        }

        public IActionResult Statics()
        {
            var productStats = _context
                .Products.Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Favorites)
                .Select(p => new ProductStatsViewModel
                {
                    ProductName = p.Name,
                    CategoryName = p.Category.Name,
                    ReviewCount = p.Reviews.Count,
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                    FavoriteCount = p.Favorites.Count,
                })
                .ToList();

            var categoryOrderStats = _context
                .OrderItems.Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(oi => oi.Product.Category.Name)
                .Select(g => new CategoryOrderStatViewModel
                {
                    CategoryName = g.Key,
                    TotalOrders = g.Sum(oi => oi.Quantity),
                })
                .OrderByDescending(c => c.TotalOrders)
                .ToList();
            var returnReasons = _context
                .ReturnRequests.GroupBy(r => r.Reason)
                .Select(g => new ReturnReasonStatViewModel { Reason = g.Key, Count = g.Count() })
                .OrderByDescending(r => r.Count)
                .ToList();

            var productReturns = _context
                .ReturnRequests.Include(r => r.Order)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Product)
                .SelectMany(r => r.Order.Items.Select(i => i.Product.Name))
                .GroupBy(p => p)
                .Select(g => new ProductReturnStatViewModel
                {
                    ProductName = g.Key,
                    ReturnCount = g.Count(),
                })
                .OrderByDescending(p => p.ReturnCount)
                .ToList();

            var viewModel = new AdminDashboardViewModel
            {
                ProductStats = productStats,
                CategoryOrderStats = categoryOrderStats,
                ProductReturns = productReturns,
                ReturnReasons = returnReasons,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Categories() =>
            View(await _context.Categories.ToListAsync());

        public async Task<IActionResult> Products() =>
            View(await _context.Products.Include(p => p.Category).ToListAsync());

        public async Task<IActionResult> Orders()
        {
            var orders = await _context
                .Orders.Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Users() => View(await _userManager.Users.ToListAsync());

        public async Task<IActionResult> Campaigns()
        {
            var campaigns = await _context.Campaigns.Include(c => c.Products).ToListAsync();
            return View(campaigns);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReturnRequests()
        {
            var requests = await _context
                .ReturnRequests.Include(r => r.Order)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReturn(int id)
        {
            var request = await _context
                .ReturnRequests.Include(r => r.Order)
                .ThenInclude(o => o.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound();

            if (!request.IsApproved)
            {
                request.IsApproved = true;
                request.ApprovalDate = DateTime.Now;

                foreach (var item in request.Order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                        product.Stock += item.Quantity;
                }

                if (request.Order != null)
                {
                    request.Order.Status = OrderStatus.Returned;
                    _context.Orders.Update(request.Order);
                }

                await _context.SaveChangesAsync();

                _context.UserMessages.Add(
                    new UserMessage
                    {
                        UserId = request.Order.UserId,
                        Message =
                            $"Siparişinizin iade talebi onaylandı. Sipariş No: {request.OrderId}",
                    }
                );

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ReturnRequests));
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context
                .Orders.Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }
    }
}
