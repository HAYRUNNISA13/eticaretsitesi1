using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;
using System.Security.Claims;

public class NavBarStatsViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public NavBarStatsViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int cartCount = 0;
        int favoriteCount = 0;

        var claimsPrincipal = User as ClaimsPrincipal;

        if (claimsPrincipal?.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
        {
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                cartCount = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .SumAsync(ci => ci.Quantity);

                favoriteCount = await _context.Favorites
                    .CountAsync(f => f.UserId == userId);
            }
        }

        ViewBag.CartCount = cartCount;
        ViewBag.FavoriteCount = favoriteCount;

        return View();
    }
}
