using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KFCWebApp.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Promotion
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var promotions = await _context.Promotions
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
            return View(promotions);
        }
    }
} 