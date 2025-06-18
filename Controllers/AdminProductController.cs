using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KFCWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    product.CreatedAt = DateTime.Now;
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm sản phẩm: " + ex.Message);
                }
            }
            
            // Nếu có lỗi, load lại categories và hiển thị lỗi
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message);
                }
            }
            
            // Nếu có lỗi, load lại categories và hiển thị lỗi
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                try
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Xóa sản phẩm thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message;
                }
            }
            return RedirectToAction("Index");
        }
    }
} 