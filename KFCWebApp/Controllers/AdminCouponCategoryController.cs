using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Threading.Tasks;

namespace KFCWebApp.Controllers
{
    public class AdminCouponCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminCouponCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminCouponCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _context.CouponCategories.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View(categories);
        }

        // GET: AdminCouponCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.CouponCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: AdminCouponCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminCouponCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponCategory category)
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

        // GET: AdminCouponCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.CouponCategories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: AdminCouponCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponCategory category)
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

        // GET: AdminCouponCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.CouponCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: AdminCouponCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.CouponCategories.FindAsync(id);
            if (category != null)
            {
                _context.CouponCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.CouponCategories.Any(e => e.Id == id);
        }
    }
} 