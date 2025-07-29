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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,CategoryId,ImageUrl,IsAvailable,OptionType")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem sản phẩm đã tồn tại chưa
                    var existingProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name.ToLower() == product.Name.ToLower());
                    
                    if (existingProduct != null)
                    {
                        ModelState.AddModelError("Name", "Sản phẩm với tên này đã tồn tại!");
                        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                        return View(product);
                    }

                    product.CreatedAt = DateTime.Now;
                    product.UpdatedAt = DateTime.Now;
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
            if (product == null) 
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Index");
            }
            
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem sản phẩm có tồn tại không
                    var existingProduct = await _context.Products.FindAsync(product.Id);
                    if (existingProduct == null)
                    {
                        TempData["Error"] = "Không tìm thấy sản phẩm!";
                        return RedirectToAction("Index");
                    }

                    // Kiểm tra xem tên sản phẩm đã tồn tại ở sản phẩm khác chưa
                    var duplicateProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name.ToLower() == product.Name.ToLower() && p.Id != product.Id);
                    
                    if (duplicateProduct != null)
                    {
                        ModelState.AddModelError("Name", "Sản phẩm với tên này đã tồn tại!");
                        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                        return View(product);
                    }

                    // Cập nhật thông tin sản phẩm
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.IsAvailable = product.IsAvailable;
                    existingProduct.OptionType = product.OptionType;
                    existingProduct.UpdatedAt = DateTime.Now;

                    _context.Products.Update(existingProduct);
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
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) 
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                try
                {
                    // Kiểm tra xem sản phẩm có trong đơn hàng nào không
                    var hasOrders = await _context.OrderDetails.AnyAsync(od => od.ProductId == id);
                    if (hasOrders)
                    {
                        TempData["Error"] = "Không thể xóa sản phẩm vì đã có trong đơn hàng!";
                        return RedirectToAction("Index");
                    }

                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Xóa sản phẩm thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
            }
            return RedirectToAction("Index");
        }
    }
} 