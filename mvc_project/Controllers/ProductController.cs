using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;
            return View(await products.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Campaigns = await _context.Campaigns.ToListAsync();

            var suppliers = new List<AppUser>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Supplier"))
                {
                    suppliers.Add(user);
                }
            }
            ViewBag.Suppliers = suppliers;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            var supplierIdValue = product.SupplierId;

            Console.WriteLine($"Gönderilen SupplierId: '{supplierIdValue}'");

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Products", "Admin");
            }

            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            foreach (var err in errors)
            {
                Console.WriteLine("Model Error: " + err);
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Campaigns = await _context.Campaigns.ToListAsync();

            var suppliers = new List<AppUser>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Supplier"))
                {
                    suppliers.Add(user);
                }
            }
            ViewBag.Suppliers = suppliers;

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Campaigns = await _context.Campaigns.ToListAsync();

            var suppliers = new List<AppUser>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Supplier"))
                {
                    suppliers.Add(user);
                }
            }
            ViewBag.Suppliers = suppliers;

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Products", "Admin");
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Campaigns = await _context.Campaigns.ToListAsync();

            var suppliers = new List<AppUser>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Supplier"))
                {
                    suppliers.Add(user);
                }
            }
            ViewBag.Suppliers = suppliers;

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Products", "Admin");
        }

        private bool HasPurchasedProduct(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            return _context
                .Orders.Include(o => o.Items)
                .Any(o => o.UserId == userId && o.Items.Any(i => i.ProductId == productId));
        }

        [HttpGet]
        public async Task<IActionResult> Review(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasPurchased = HasPurchasedProduct(userId, productId);

            if (!hasPurchased)
                return Forbid();

            var model = new ProductReview { ProductId = productId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Review(ProductReview review)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasPurchased = HasPurchasedProduct(userId, review.ProductId);

            if (!hasPurchased)
                return Forbid();

            review.UserId = userId;
            review.CreatedAt = DateTime.Now;

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Product", new { id = review.ProductId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context
                .Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var reviews = await _context
                .ProductReviews.Include(r => r.User)
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Reviews = reviews;
            ViewBag.AvgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var favs = await _context
                    .Favorites.Where(f => f.UserId == userId)
                    .Select(f => f.ProductId)
                    .ToListAsync();

                ViewBag.Favorites = favs;
                ViewBag.HasPurchased = HasPurchasedProduct(userId, product.Id);
            }
            else
            {
                ViewBag.Favorites = new List<int>();
                ViewBag.HasPurchased = false;
            }

            return View(product);
        }

        public async Task<IActionResult> Search(string? q, int? categoryId)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(q))
                products = products.Where(p =>
                    p.Name.Contains(q) || (p.Description != null && p.Description.Contains(q))
                );

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId);

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Query = q;

            return View("Index", await products.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> QuickView(int id)
        {
            var product = await _context
                .Products.Include(p => p.Category)
                .Include(p => p.Campaign)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var hasCampaign =
                product.Campaign != null
                && product.Campaign.DiscountRate > 0
                && product.Campaign.StartDate <= DateTime.Now
                && product.Campaign.EndDate >= DateTime.Now;

            var discountedPrice = hasCampaign
                ? product.Price * (1 - (product.Campaign?.DiscountRate ?? 0))
                : product.Price;

            ViewBag.HasCampaign = hasCampaign;
            ViewBag.DiscountedPrice = discountedPrice;

            return PartialView("_QuickView", product);
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var favorites = await _context
                .Favorites.Where(f => f.UserId == userId)
                .Include(f => f.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.UserId == userId && f.ProductId == productId
            );

            if (favorite == null)
            {
                favorite = new Favorite { UserId = userId, ProductId = productId };
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                return Json(new { added = true });
            }
            else
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { added = false });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.UserId == userId && f.ProductId == productId
            );

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Favorites");
        }
    }
}
