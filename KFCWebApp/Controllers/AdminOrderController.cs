using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using KFCWebApp.Extensions;
using KFCWebApp.Services;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Globalization;

namespace KFCWebApp.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminOrderController> _logger;
        private readonly PdfService _pdfService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminOrderController(
            ApplicationDbContext context, 
            ILogger<AdminOrderController> logger,
            PdfService pdfService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _pdfService = pdfService;
            _userManager = userManager;
        }

        // GET: AdminOrder
        public async Task<IActionResult> Index(string searchString, string status)
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.OrderDetails)
                    .AsQueryable();

                // Tìm kiếm theo tên khách hàng hoặc số điện thoại (case-insensitive)
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    orders = orders.Where(o => 
                        o.CustomerName.ToLower().Contains(searchString) || 
                        o.PhoneNumber.ToLower().Contains(searchString));
                }

                // Lọc theo trạng thái
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    orders = orders.Where(o => o.Status == orderStatus);
                }

                ViewBag.CurrentSearch = searchString;
                ViewBag.CurrentStatus = status;
                ViewBag.Statuses = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(s => new { Value = s.ToString(), Text = s.GetDisplayName() });

                return View(await orders.OrderByDescending(o => o.CreatedAt).ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đơn hàng");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách đơn hàng";
                return View(new List<Order>());
            }
        }

        // GET: AdminOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết đơn hàng {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin đơn hàng";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AdminOrder/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                // Kiểm tra tính hợp lệ của trạng thái mới
                if (!IsValidStatusTransition(order.Status, status))
                {
                    TempData["Error"] = "Không thể chuyển đổi sang trạng thái này";
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }

                order.Status = status;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["Message"] = "Cập nhật trạng thái đơn hàng thành công!";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: AdminOrder/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
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
                    TempData["Error"] = "Không thể hủy đơn hàng này!";
                }

                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy đơn hàng {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi hủy đơn hàng";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Định nghĩa các quy tắc chuyển đổi trạng thái hợp lệ
            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Processing || 
                           newStatus == OrderStatus.Cancelled;
                case OrderStatus.Processing:
                    return newStatus == OrderStatus.Completed || 
                           newStatus == OrderStatus.Cancelled;
                case OrderStatus.Completed:
                    return false; // Không thể chuyển từ Completed sang trạng thái khác
                case OrderStatus.Cancelled:
                    return false; // Không thể chuyển từ Cancelled sang trạng thái khác
                default:
                    return false;
            }
        }

        // GET: AdminOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem trang xóa đơn hàng {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin đơn hàng";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AdminOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem có thể xóa đơn hàng không
                if (order.Status == OrderStatus.Completed)
                {
                    TempData["Error"] = "Không thể xóa đơn hàng đã hoàn thành!";
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }

                // Xóa các OrderDetails trước
                _context.OrderDetails.RemoveRange(order.OrderDetails);
                
                // Xóa đơn hàng
                _context.Orders.Remove(order);
                
                await _context.SaveChangesAsync();

                TempData["Message"] = "Đã xóa đơn hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa đơn hàng {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi xóa đơn hàng";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: AdminOrder/DownloadPdf/5
        public async Task<IActionResult> DownloadPdf(int? id)
        {
            try
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

                var pdfBytes = _pdfService.GenerateOrderPdf(order);
                return File(pdfBytes, "application/pdf", $"DonHang_{order.Id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo PDF cho đơn hàng {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi tạo file PDF";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // Báo cáo doanh thu
        [HttpGet]
        public IActionResult RevenueReport()
        {
            return View();
        }

        // Tạo data giả cho testing
        [HttpPost]
        public async Task<IActionResult> CreateSampleData()
        {
            try
            {
                var adminUser = await _userManager.FindByEmailAsync("admin@kfc.com");
                if (adminUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy admin user" });
                }

                var products = await _context.Products.ToListAsync();
                var combos = await _context.Combos.ToListAsync();
                
                if (!products.Any() || !combos.Any())
                {
                    return Json(new { success = false, message = "Chưa có sản phẩm hoặc combo" });
                }

                var random = new Random();
                var orders = new List<Order>();
                
                // Tạo 30 đơn hàng trong 30 ngày gần đây
                for (int i = 0; i < 30; i++)
                {
                    var orderDate = DateTime.Now.AddDays(-random.Next(0, 30));
                    var totalAmount = random.Next(50000, 500000);
                    
                    var order = new Order
                    {
                        ApplicationUserId = adminUser.Id,
                        CustomerName = $"Khách hàng {i + 1}",
                        Email = $"customer{i + 1}@example.com",
                        PhoneNumber = $"0{random.Next(900000000, 999999999)}",
                        Address = $"Địa chỉ {i + 1}, TP.HCM",
                        TotalAmount = totalAmount,
                        Status = OrderStatus.Completed,
                        CreatedAt = orderDate,
                        UpdatedAt = orderDate
                    };
                    orders.Add(order);
                }

                _context.Orders.AddRange(orders);
                await _context.SaveChangesAsync();

                // Tạo OrderDetails cho mỗi đơn hàng
                var orderDetails = new List<OrderDetail>();
                foreach (var order in orders)
                {
                    var itemCount = random.Next(1, 4); // 1-3 món mỗi đơn
                    for (int j = 0; j < itemCount; j++)
                    {
                        var isCombo = random.Next(2) == 0; // 50% combo, 50% sản phẩm
                        var quantity = isCombo ? random.Next(1, 3) : random.Next(1, 4);
                        
                        if (isCombo)
                        {
                            var combo = combos[random.Next(combos.Count)];
                            orderDetails.Add(new OrderDetail
                            {
                                OrderId = order.Id,
                                ComboId = combo.Id,
                                ItemName = combo.Name,
                                ItemType = "Combo",
                                Quantity = quantity,
                                Price = combo.Price
                            });
                        }
                        else
                        {
                            var product = products[random.Next(products.Count)];
                            orderDetails.Add(new OrderDetail
                            {
                                OrderId = order.Id,
                                ProductId = product.Id,
                                ItemName = product.Name,
                                ItemType = "Product",
                                Quantity = quantity,
                                Price = product.Price
                            });
                        }
                    }
                }

                _context.OrderDetails.AddRange(orderDetails);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã tạo {orders.Count} đơn hàng mẫu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // API lấy dữ liệu doanh thu theo ngày
        [HttpGet]
        public async Task<IActionResult> GetRevenueData(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = _context.Orders
                    .Where(o => o.Status == OrderStatus.Completed); // Chỉ tính đơn đã hoàn thành
                
                if (fromDate.HasValue)
                    query = query.Where(o => o.CreatedAt >= fromDate.Value);
                if (toDate.HasValue)
                    query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1).AddSeconds(-1)); // Bao gồm cả ngày cuối
                
                // Lấy dữ liệu từ database trước
                var rawData = await query
                    .GroupBy(o => o.CreatedAt.Date)
                    .Select(g => new {
                        Date = g.Key,
                        Total = g.Sum(x => x.TotalAmount),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();
                
                // Format date trong memory
                var data = rawData.Select(g => new {
                    Date = g.Date.ToString("yyyy-MM-dd"),
                    Total = g.Total,
                    OrderCount = g.OrderCount
                }).ToList();
                
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu doanh thu");
                return Json(new List<object>());
            }
        }
    }
} 