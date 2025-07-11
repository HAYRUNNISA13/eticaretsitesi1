using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;
using System.Security.Claims;



public class NavBarMessagesViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public NavBarMessagesViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return View(new List<UserMessage>());
        }

        var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var messages = await _context.UserMessages
            .Where(m => m.UserId == userId && !m.IsRead)
            .OrderByDescending(m => m.CreatedAt)
            .Take(5)
            .ToListAsync();

        return View(messages);
    }
}
