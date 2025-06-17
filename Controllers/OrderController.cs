using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Text.Json;

namespace KFCWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Checkout
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            return View(new Order());
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("CustomerName,PhoneNumber,Address,Email,Note")] Order order)
        {
            if (ModelState.IsValid)
            {
                var cart = GetCart();
                if (!cart.Any())
                {
                    TempData["Message"] = "Giỏ hàng của bạn đang trống!";
                    return RedirectToAction("Index", "Cart");
                }

                // Kiểm tra sản phẩm trong giỏ hàng
                foreach (var item in cart)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        TempData["Message"] = $"Sản phẩm {item.ProductName} không tồn tại!";
                        return RedirectToAction("Index", "Cart");
                    }
                    if (!product.IsAvailable)
                    {
                        TempData["Message"] = $"Sản phẩm {item.ProductName} hiện không có sẵn!";
                        return RedirectToAction("Index", "Cart");
                    }
                }

                // Tạo đơn hàng mới
                order.TotalAmount = cart.Sum(i => i.Total);
                order.Status = OrderStatus.Pending;
                order.CreatedAt = DateTime.Now;

                _context.Add(order);
                await _context.SaveChangesAsync();

                // Thêm chi tiết đơn hàng
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity
                    };
                    _context.Add(orderDetail);
                }
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng
                HttpContext.Session.Remove(CartSessionKey);

                TempData["Message"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }

            return View(order);
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã hủy đơn hàng thành công!";
            }
            else
            {
                TempData["Message"] = "Không thể hủy đơn hàng này!";
            }

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }
    }
} 