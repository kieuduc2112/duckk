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
    public class AdminProductPromotionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminProductPromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminProductPromotion
        public async Task<IActionResult> Index()
        {
            var promotions = await _context.ProductPromotions.Include(p => p.Product1).Include(p => p.Product2).OrderByDescending(p => p.StartDate).ToListAsync();
            return View(promotions);
        }

        // GET: AdminProductPromotion/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View();
        }

        // POST: AdminProductPromotion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductPromotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(promotion);
        }

        // GET: AdminProductPromotion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.ProductPromotions.FindAsync(id);
            if (promotion == null) return NotFound();
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(promotion);
        }

        // POST: AdminProductPromotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductPromotion promotion)
        {
            if (id != promotion.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _context.Products.Where(p => p.IsAvailable).ToListAsync();
            return View(promotion);
        }

        // GET: AdminProductPromotion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.ProductPromotions.Include(p => p.Product1).Include(p => p.Product2).FirstOrDefaultAsync(p => p.Id == id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }

        // POST: AdminProductPromotion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _context.ProductPromotions.FindAsync(id);
            if (promotion != null)
            {
                _context.ProductPromotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminProductPromotion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.ProductPromotions
                .Include(p => p.Product1)
                .Include(p => p.Product2)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }
    }
} 