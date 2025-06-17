using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using KFCWebApp.Extensions;
using KFCWebApp.Services;
using System.Data;

namespace KFCWebApp.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminOrderController> _logger;
        private readonly PdfService _pdfService;

        public AdminOrderController(
            ApplicationDbContext context, 
            ILogger<AdminOrderController> logger,
            PdfService pdfService)
        {
            _context = context;
            _logger = logger;
            _pdfService = pdfService;
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
    }
} 