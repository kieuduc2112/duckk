using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KFCWebApp.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Coupon
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var coupons = await _context.Coupons
                .Where(c => c.IsActive && c.Quantity > 0 && c.StartDate <= now && c.EndDate >= now)
                .OrderByDescending(c => c.EndDate)
                .ToListAsync();
            return View(coupons);
        }
    }
} 