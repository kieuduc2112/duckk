using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Text.Json;
using KFCWebApp.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;

namespace KFCWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";
        private readonly PayOSService _payosService;
        private readonly IConfiguration _config;

        public OrderController(ApplicationDbContext context, PayOSService payosService, IConfiguration config)
        {
            _context = context;
            _payosService = payosService;
            _config = config;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Có thể chuyển hướng về trang đăng nhập hoặc trả về view rỗng
                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var orders = await _context.Orders
                .Where(o => o.ApplicationUserId == userId)
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
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            ViewBag.Cart = cart;
            decimal subtotal = cart.Where(x => x.ItemType == "Product" || x.ItemType == "Combo").Sum(x => x.Price * x.Quantity);
            ViewBag.Total = subtotal;
            ViewBag.VoucherDiscount = 0m;
            ViewBag.VoucherCode = null;
            ViewBag.Discount = 0m;
            ViewBag.CouponCode = "";
            return View(new OrderCheckoutViewModel());
        }

        // AJAX: Kiểm tra mã giảm giá
        [HttpGet]
        public async Task<IActionResult> CheckCoupon(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá!" });

            var now = DateTime.Now;
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code && c.IsActive && c.Quantity > 0 && c.StartDate <= now && c.EndDate >= now);
            if (coupon == null)
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hiệu lực!" });

            string msg = coupon.Type == CouponType.Percentage
                ? $"Áp dụng thành công: Giảm {coupon.Value:N0}% cho đơn hàng."
                : $"Áp dụng thành công: Giảm {coupon.Value:N0} VNĐ cho đơn hàng.";
            return Json(new { success = true, message = msg });
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderCheckoutViewModel model)
        {
            var cart = GetCart();
            Coupon? coupon = null;
            // Tổng tiền đã bao gồm khuyến mãi động (cả dòng Discount, PercentDiscount...)
            decimal cartTotal = cart.Sum(x => x.Total); // Đã trừ hết các khuyến mãi động
            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(model.VoucherCode))
            {
                var now = DateTime.Now;
                coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == model.VoucherCode && c.IsActive && c.Quantity > 0 && c.StartDate <= now && c.EndDate >= now);
                if (coupon != null)
                {
                    if (coupon.Type == CouponType.Percentage)
                        discount = Math.Round(cartTotal * coupon.Value / 100, 0);
                    else if (coupon.Type == CouponType.Amount)
                        discount = coupon.Value;
                    if (discount > cartTotal) discount = cartTotal;
                    ViewBag.VoucherDiscount = discount;
                    ViewBag.VoucherCode = model.VoucherCode;
                }
                else
                {
                    ViewBag.VoucherDiscount = 0m;
                    ViewBag.VoucherCode = null;
                }
            }
            else
            {
                ViewBag.VoucherDiscount = 0m;
                ViewBag.VoucherCode = null;
            }
            ViewBag.Total = cartTotal - (decimal)(ViewBag.VoucherDiscount ?? 0);
            if (ModelState.IsValid)
            {
                if (!cart.Any())
                {
                    TempData["Message"] = "Giỏ hàng của bạn đang trống!";
                    return RedirectToAction("Index", "Cart");
                }
                // Kiểm tra sản phẩm và combo trong giỏ hàng
                foreach (var item in cart)
                {
                    if (item.ItemType == "Product")
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product == null)
                        {
                            TempData["Message"] = $"Sản phẩm {item.ItemName} không tồn tại!";
                            return RedirectToAction("Index", "Cart");
                        }
                        if (!product.IsAvailable)
                        {
                            TempData["Message"] = $"Sản phẩm {item.ItemName} hiện không có sẵn!";
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                    else if (item.ItemType == "Combo")
                    {
                        var combo = await _context.Combos.FindAsync(item.ComboId);
                        if (combo == null)
                        {
                            TempData["Message"] = $"Combo {item.ItemName} không tồn tại!";
                            return RedirectToAction("Index", "Cart");
                        }
                        if (!combo.IsAvailable)
                        {
                            TempData["Message"] = $"Combo {item.ItemName} hiện không có sẵn!";
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                }
                // Tạo đơn hàng mới
                var order = new Order
                {
                    TotalAmount = cartTotal - (decimal)(ViewBag.VoucherDiscount ?? 0),
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.Now
                };
                _context.Add(order);
                await _context.SaveChangesAsync();
                // Thêm chi tiết đơn hàng
                var groupedDetails = cart
                    .Where(item => (item.ItemType == "Product" && item.ProductId != null) || (item.ItemType == "Combo" && item.ComboId != null))
                    .GroupBy(item => new { item.ProductId, item.ComboId, item.ItemName, item.ItemType, item.Price })
                    .Select(g => new
                    {
                        ProductId = g.Key.ProductId,
                        ComboId = g.Key.ComboId,
                        ItemName = g.Key.ItemName,
                        ItemType = g.Key.ItemType,
                        Price = g.Key.Price,
                        Quantity = g.Sum(x => x.Quantity)
                    });
                foreach (var item in groupedDetails)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ComboId = item.ComboId,
                        ItemName = item.ItemName,
                        ItemType = item.ItemType,
                        Price = item.Price,
                        Quantity = item.Quantity
                    });
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";
                var returnUrl = Url.Action("PayOSCallback", "Order", new { orderId = order.Id }, Request.Scheme);
                var payUrl = await _payosService.CreatePaymentLink(order.TotalAmount, order.Id.ToString(), returnUrl, $"Thanh toán đơn hàng #{order.Id}");
                if (payUrl != null)
                    return Redirect(payUrl);
                ModelState.AddModelError("", "Không tạo được link thanh toán online. Vui lòng thử lại.");
                return View(model);
            }
            // Nếu ModelState không hợp lệ hoặc có lỗi
            ViewBag.Cart = cart;
            ViewBag.Discount = discount;
            ViewBag.VoucherDiscount = discount;
            ViewBag.VoucherCode = model.VoucherCode;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PayOSCallback(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();
            
            // Kiểm tra trạng thái thanh toán từ PayOS
            var paymentStatus = await _payosService.GetPaymentStatus(orderId.ToString());
            if (paymentStatus == "PAID")
            {
                order.Status = Models.OrderStatus.Completed;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                
                // XÓA giỏ hàng khi thanh toán thành công
                HttpContext.Session.Remove(CartSessionKey);
                
                TempData["Message"] = "Thanh toán thành công! Cảm ơn bạn đã mua hàng.";
            }
            else
            {
                order.Status = Models.OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thanh toán không thành công hoặc đã bị hủy.";
            }
            
            return RedirectToAction("Details", new { id = orderId });
        }

        // Webhook endpoint để nhận thông báo từ PayOS
        [HttpPost]
        public async Task<IActionResult> PayOSWebhook()
        {
            try
            {
                var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                var webhookData = JsonSerializer.Deserialize<PayOSWebhookData>(requestBody);
                
                if (webhookData != null && int.TryParse(webhookData.orderId, out int orderId))
                {
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        if (webhookData.status == "PAID")
                        {
                            order.Status = Models.OrderStatus.Completed;
                            order.UpdatedAt = DateTime.Now;
                            await _context.SaveChangesAsync();
                            
                            // Log thành công
                            Console.WriteLine($"Webhook: Đơn hàng {orderId} đã được thanh toán thành công");
                        }
                        else if (webhookData.status == "CANCELLED")
                        {
                            order.Status = Models.OrderStatus.Cancelled;
                            order.UpdatedAt = DateTime.Now;
                            await _context.SaveChangesAsync();
                            
                            // Log hủy
                            Console.WriteLine($"Webhook: Đơn hàng {orderId} đã bị hủy");
                        }
                    }
                }
                
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return BadRequest();
            }
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

        [HttpGet]
        public async Task<IActionResult> CheckVoucher(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá!", discount = 0 });
            var now = DateTime.Now;
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code && c.IsActive && c.Quantity > 0 && c.StartDate <= now && c.EndDate >= now);
            if (coupon == null)
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hiệu lực!", discount = 0 });
            var cart = GetCart();
            var subtotal = cart.Sum(i => i.Total);
            decimal discount = 0;
            if (coupon.Type == CouponType.Percentage)
                discount = Math.Round(subtotal * coupon.Value / 100, 0);
            else
                discount = coupon.Value;
            if (discount > subtotal) discount = subtotal;
            string msg = coupon.Type == CouponType.Percentage
                ? $"Áp dụng thành công: Giảm {coupon.Value:N0}% cho đơn hàng."
                : $"Áp dụng thành công: Giảm {coupon.Value:N0} VNĐ cho đơn hàng.";
            return Json(new { success = true, message = msg, discount });
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            
            try
            {
                var result = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                return result ?? new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
        }
    }
} 