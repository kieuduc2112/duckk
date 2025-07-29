using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace KFCWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPromotionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminPromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminPromotion
        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions.OrderByDescending(p => p.StartDate).ToListAsync();
            return View(promotions);
        }

        // GET: AdminPromotion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
            if (promotion == null) return NotFound();

            // Nếu là TwoProductPromotion thì parse JSON lấy tên sản phẩm
            string? product1Name = null;
            string? product2Name = null;
            if (promotion.ConditionType == "TwoProductPromotion")
            {
                try
                {
                    var cond = JsonSerializer.Deserialize<Dictionary<string, string>>(promotion.ConditionJson);
                    if (cond != null && cond.ContainsKey("ProductId1") && cond.ContainsKey("ProductId2"))
                    {
                        if (int.TryParse(cond["ProductId1"], out int pid1))
                        {
                            var p1 = await _context.Products.FindAsync(pid1);
                            product1Name = p1?.Name;
                        }
                        if (int.TryParse(cond["ProductId2"], out int pid2))
                        {
                            var p2 = await _context.Products.FindAsync(pid2);
                            product2Name = p2?.Name;
                        }
                    }
                }
                catch { }
            }
            ViewBag.Product1Name = product1Name;
            ViewBag.Product2Name = product2Name;
            return View(promotion);
        }

        // GET: AdminPromotion/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(new Promotion { ConditionType = "TwoProductPromotion" });
        }

        // POST: AdminPromotion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(promotion);
        }

        // GET: AdminPromotion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(promotion);
        }

        // POST: AdminPromotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promotion promotion)
        {
            if (id != promotion.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(promotion);
        }

        // GET: AdminPromotion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }

        // POST: AdminPromotion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 