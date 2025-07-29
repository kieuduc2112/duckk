using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace KFCWebApp.Controllers
{
    public class AdminCouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminCouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminCoupon
        public async Task<IActionResult> Index()
        {
            var coupons = await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View(coupons);
        }

        // GET: AdminCoupon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null) return NotFound();
            return View(coupon);
        }

        // GET: AdminCoupon/Create
        public IActionResult Create()
        {
            ViewBag.CouponCategories = _context.CouponCategories.Where(c => c.IsActive).ToList();
            return View();
        }

        // POST: AdminCoupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                coupon.IsActive = true;
                coupon.CreatedAt = DateTime.Now;
                _context.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CouponCategories = _context.CouponCategories.Where(c => c.IsActive).ToList();
            return View(coupon);
        }

        // GET: AdminCoupon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            ViewBag.CouponCategories = _context.CouponCategories.Where(c => c.IsActive).ToList();
            return View(coupon);
        }

        // POST: AdminCoupon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (id != coupon.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CouponCategories = _context.CouponCategories.Where(c => c.IsActive).ToList();
            return View(coupon);
        }

        // GET: AdminCoupon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null) return NotFound();
            return View(coupon);
        }

        // POST: AdminCoupon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
} 