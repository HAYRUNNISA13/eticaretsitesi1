using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    public class CampaignController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CampaignController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var campaigns = await _context.Campaigns.ToListAsync();
            return View(campaigns);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                _context.Campaigns.Add(campaign);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(campaign);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                return NotFound();
            return View(campaign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Campaign campaign)
        {
            if (id != campaign.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(campaign);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(campaign);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                return NotFound();
            return View(campaign);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var campaign = await _context
                .Campaigns.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (campaign != null)
            {
                if (campaign.Products != null)
                {
                    foreach (var product in campaign.Products)
                    {
                        product.CampaignId = null;
                    }
                }
                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
