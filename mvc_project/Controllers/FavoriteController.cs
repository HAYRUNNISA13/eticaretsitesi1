using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FavoriteController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorites = await _context
                .Favorites.Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToListAsync();
            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_context.Favorites.Any(f => f.UserId == userId && f.ProductId == productId))
            {
                _context.Favorites.Add(new Favorite { UserId = userId!, ProductId = productId });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fav = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.UserId == userId && f.ProductId == productId
            );
            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}
