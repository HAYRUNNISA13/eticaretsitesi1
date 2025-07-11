using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mesajları gösterir
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _context
                .UserMessages.Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Okunmamışları okundu olarak işaretle
            foreach (var msg in messages.Where(m => !m.IsRead))
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userMessages = await _context
                .UserMessages.Where(m => m.UserId == userId)
                .ToListAsync();

            _context.UserMessages.RemoveRange(userMessages);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Tüm mesajlar silindi.";
            return RedirectToAction("Index");
        }
    }
}
