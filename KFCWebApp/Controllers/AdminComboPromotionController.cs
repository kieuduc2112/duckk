using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Threading.Tasks;
using System.Linq;

namespace KFCWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminComboPromotionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminComboPromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminComboPromotion
        public async Task<IActionResult> Index()
        {
            var combos = await _context.ComboPromotions.Include(c => c.Items).OrderByDescending(c => c.StartDate).ToListAsync();
            return View(combos);
        }

        // GET: AdminComboPromotion/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View();
        }

        // POST: AdminComboPromotion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComboPromotion combo, int[] productIds, int[] quantities)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < productIds.Length; i++)
                {
                    combo.Items.Add(new ComboPromotionItem { ProductId = productIds[i], Quantity = quantities[i] });
                }
                _context.Add(combo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(combo);
        }

        // GET: AdminComboPromotion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var combo = await _context.ComboPromotions.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
            if (combo == null) return NotFound();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(combo);
        }

        // POST: AdminComboPromotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComboPromotion combo, int[] productIds, int[] quantities)
        {
            if (id != combo.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var oldCombo = await _context.ComboPromotions.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
                if (oldCombo == null) return NotFound();
                oldCombo.Name = combo.Name;
                oldCombo.Description = combo.Description;
                oldCombo.StartDate = combo.StartDate;
                oldCombo.EndDate = combo.EndDate;
                oldCombo.IsActive = combo.IsActive;
                oldCombo.RewardType = combo.RewardType;
                oldCombo.RewardValue = combo.RewardValue;
                oldCombo.RewardProductId = combo.RewardProductId;
                oldCombo.Items.Clear();
                for (int i = 0; i < productIds.Length; i++)
                {
                    oldCombo.Items.Add(new ComboPromotionItem { ProductId = productIds[i], Quantity = quantities[i] });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(combo);
        }

        // GET: AdminComboPromotion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var combo = await _context.ComboPromotions.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
            if (combo == null) return NotFound();
            return View(combo);
        }

        // POST: AdminComboPromotion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var combo = await _context.ComboPromotions.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
            if (combo != null)
            {
                _context.ComboPromotionItems.RemoveRange(combo.Items);
                _context.ComboPromotions.Remove(combo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 