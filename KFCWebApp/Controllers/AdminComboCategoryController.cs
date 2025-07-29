using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;

namespace KFCWebApp.Controllers
{
    public class AdminComboCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminComboCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminComboCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _context.ComboCategories.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View(categories);
        }

        // GET: AdminComboCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.ComboCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: AdminComboCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminComboCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComboCategory category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: AdminComboCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.ComboCategories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: AdminComboCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComboCategory category)
        {
            if (id != category.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    category.UpdatedAt = DateTime.Now;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: AdminComboCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.ComboCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: AdminComboCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.ComboCategories.FindAsync(id);
            if (category != null)
            {
                _context.ComboCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.ComboCategories.Any(e => e.Id == id);
        }
    }
} 