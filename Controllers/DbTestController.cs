using Microsoft.AspNetCore.Mvc;
using KFCWebApp.Data;

namespace KFCWebApp.Controllers
{
    public class DbTestController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DbTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var count = _context.Products.Count();
                return Content($"Kết nối thành công! Số sản phẩm: {count}");
            }
            catch (Exception ex)
            {
                return Content($"Kết nối thất bại: {ex.Message}");
            }
        }
    }
} 