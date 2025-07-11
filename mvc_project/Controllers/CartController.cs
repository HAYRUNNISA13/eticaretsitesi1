using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context
                .Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["CartMessage"] = "Sepete ürün eklemek için giriş yapmalısınız.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = await _context
                    .Carts.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                    _context.Carts.Add(cart);
                }
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                    item.Quantity += quantity;
                else
                    cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCampaign(int campaignId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context
                .Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var campaign = await _context
                .Campaigns.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            if (cart == null || campaign == null)
            {
                return RedirectToAction("Index");
            }

            foreach (var item in cart.Items)
            {
                if (campaign.Products.Any(p => p.Id == item.ProductId))
                {
                    item.Product.Price =
                        item.Product.Price - (item.Product.Price * campaign.DiscountRate);
                }
            }

            await _context.SaveChangesAsync();
            TempData["CampaignMessage"] = $"“{campaign.Name}” kampanyası sepetinize uygulandı!";
            return RedirectToAction("Index");
        }
    }
}
