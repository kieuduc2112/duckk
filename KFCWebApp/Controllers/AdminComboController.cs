using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;

namespace KFCWebApp.Controllers
{
    public class AdminComboController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminComboController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminCombo
        public async Task<IActionResult> Index()
        {
            var combos = await _context.Combos
                .Include(c => c.ComboCategory)
                .Include(c => c.ComboItems).ThenInclude(ci => ci.Product)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(combos);
        }

        // GET: AdminCombo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var combo = await _context.Combos
                .Include(c => c.ComboCategory)
                .Include(c => c.ComboItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (combo == null) return NotFound();
            return View(combo);
        }

        // GET: AdminCombo/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ComboCategories = await _context.ComboCategories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View();
        }

        // POST: AdminCombo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Combo combo, int[] productIds, int[] quantities)
        {
            if (ModelState.IsValid)
            {
                combo.CreatedAt = DateTime.Now;
                _context.Add(combo);
                await _context.SaveChangesAsync();
                // Thêm các sản phẩm vào combo
                for (int i = 0; i < productIds.Length; i++)
                {
                    var comboItem = new ComboItem
                    {
                        ComboId = combo.Id,
                        ProductId = productIds[i],
                        Quantity = quantities[i]
                    };
                    _context.ComboItems.Add(comboItem);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ComboCategories = await _context.ComboCategories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(combo);
        }

        // GET: AdminCombo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var combo = await _context.Combos
                .Include(c => c.ComboItems)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (combo == null) return NotFound();
            ViewBag.ComboCategories = await _context.ComboCategories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(combo);
        }

        // POST: AdminCombo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Combo combo, int[] productIds, int[] quantities)
        {
            if (id != combo.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    combo.UpdatedAt = DateTime.Now;
                    _context.Update(combo);
                    // Xóa các ComboItem cũ
                    var oldItems = _context.ComboItems.Where(ci => ci.ComboId == combo.Id);
                    _context.ComboItems.RemoveRange(oldItems);
                    // Thêm lại các ComboItem mới
                    for (int i = 0; i < productIds.Length; i++)
                    {
                        var comboItem = new ComboItem
                        {
                            ComboId = combo.Id,
                            ProductId = productIds[i],
                            Quantity = quantities[i]
                        };
                        _context.ComboItems.Add(comboItem);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComboExists(combo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ComboCategories = await _context.ComboCategories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(combo);
        }

        // GET: AdminCombo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var combo = await _context.Combos
                .Include(c => c.ComboCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (combo == null) return NotFound();
            return View(combo);
        }

        // POST: AdminCombo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo != null)
            {
                // Xóa các ComboItem liên quan
                var items = _context.ComboItems.Where(ci => ci.ComboId == combo.Id);
                _context.ComboItems.RemoveRange(items);
                _context.Combos.Remove(combo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ComboExists(int id)
        {
            return _context.Combos.Any(e => e.Id == id);
        }
    }
} 