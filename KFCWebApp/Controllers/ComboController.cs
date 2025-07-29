using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;

namespace KFCWebApp.Controllers
{
    public class ComboController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComboController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Combo
        public async Task<IActionResult> Index(int? categoryId)
        {
            var query = _context.Combos
                .Include(c => c.ComboCategory)
                .Include(c => c.ComboItems).ThenInclude(ci => ci.Product)
                .Where(c => c.IsAvailable);

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.ComboCategoryId == categoryId);
            }

            var combos = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.Categories = await _context.ComboCategories.Where(c => c.IsActive).ToListAsync();
            ViewBag.SelectedCategoryId = categoryId;
            return View(combos);
        }

        // GET: Combo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var combo = await _context.Combos
                .Include(c => c.ComboCategory)
                .Include(c => c.ComboItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);
            if (combo == null) return NotFound();
            return View(combo);
        }
    }
} 