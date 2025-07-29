using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using KFCWebApp.Models;

namespace KFCWebApp.Services
{
    public class PdfService
    {
        public byte[] GenerateOrderPdf(Order order)
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Thêm font Unicode
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Tiêu đề
            var title = new Paragraph($"ĐƠN HÀNG #{order.Id}")
                .SetFont(font)
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold();
            document.Add(title);

            // Thông tin khách hàng
            document.Add(new Paragraph("Thông tin khách hàng").SetFont(font).SetFontSize(14).SetBold());
            var customerInfo = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetMarginTop(10)
                .SetMarginBottom(10);

            customerInfo.AddCell(new Cell().Add(new Paragraph("Họ tên:").SetFont(font)).SetBold());
            customerInfo.AddCell(new Cell().Add(new Paragraph(order.CustomerName).SetFont(font)));
            customerInfo.AddCell(new Cell().Add(new Paragraph("Số điện thoại:").SetFont(font)).SetBold());
            customerInfo.AddCell(new Cell().Add(new Paragraph(order.PhoneNumber).SetFont(font)));
            customerInfo.AddCell(new Cell().Add(new Paragraph("Email:").SetFont(font)).SetBold());
            customerInfo.AddCell(new Cell().Add(new Paragraph(order.Email).SetFont(font)));
            customerInfo.AddCell(new Cell().Add(new Paragraph("Địa chỉ:").SetFont(font)).SetBold());
            customerInfo.AddCell(new Cell().Add(new Paragraph(order.Address).SetFont(font)));
            if (!string.IsNullOrEmpty(order.Note))
            {
                customerInfo.AddCell(new Cell().Add(new Paragraph("Ghi chú:").SetFont(font)).SetBold());
                customerInfo.AddCell(new Cell().Add(new Paragraph(order.Note).SetFont(font)));
            }
            document.Add(customerInfo);

            // Chi tiết đơn hàng
            document.Add(new Paragraph("Chi tiết đơn hàng").SetFont(font).SetFontSize(14).SetBold());
            var orderTable = new Table(4)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetMarginTop(10)
                .SetMarginBottom(10);

            // Header
            var headers = new[] { "Sản phẩm", "Đơn giá", "Số lượng", "Thành tiền" };
            foreach (var header in headers)
            {
                orderTable.AddHeaderCell(new Cell().Add(new Paragraph(header).SetFont(font)).SetBold().SetTextAlignment(TextAlignment.CENTER));
            }

            // Chi tiết sản phẩm
            foreach (var item in order.OrderDetails)
            {
                orderTable.AddCell(new Cell().Add(new Paragraph(item.ItemName).SetFont(font)));
                orderTable.AddCell(new Cell().Add(new Paragraph(item.Price.ToString("N0") + " VNĐ").SetFont(font)).SetTextAlignment(TextAlignment.RIGHT));
                orderTable.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString()).SetFont(font)).SetTextAlignment(TextAlignment.CENTER));
                orderTable.AddCell(new Cell().Add(new Paragraph(item.Total.ToString("N0") + " VNĐ").SetFont(font)).SetTextAlignment(TextAlignment.RIGHT));
            }

            // Tổng tiền
            orderTable.AddCell(new Cell(1, 3).Add(new Paragraph("Tổng cộng:").SetFont(font)).SetBold().SetTextAlignment(TextAlignment.RIGHT));
            orderTable.AddCell(new Cell().Add(new Paragraph(order.TotalAmount.ToString("N0") + " VNĐ").SetFont(font)).SetBold().SetTextAlignment(TextAlignment.RIGHT));

            document.Add(orderTable);

            // Thông tin đơn hàng
            document.Add(new Paragraph("Thông tin đơn hàng").SetFont(font).SetFontSize(14).SetBold());
            var orderInfo = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetMarginTop(10);

            orderInfo.AddCell(new Cell().Add(new Paragraph("Ngày đặt:").SetFont(font)).SetBold());
            orderInfo.AddCell(new Cell().Add(new Paragraph(order.CreatedAt.ToString("dd/MM/yyyy HH:mm")).SetFont(font)));
            if (order.UpdatedAt.HasValue)
            {
                orderInfo.AddCell(new Cell().Add(new Paragraph("Cập nhật lần cuối:").SetFont(font)).SetBold());
                orderInfo.AddCell(new Cell().Add(new Paragraph(order.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")).SetFont(font)));
            }
            orderInfo.AddCell(new Cell().Add(new Paragraph("Trạng thái:").SetFont(font)).SetBold());
            orderInfo.AddCell(new Cell().Add(new Paragraph(order.Status.ToString()).SetFont(font)));

            document.Add(orderInfo);

            document.Close();
            return memoryStream.ToArray();
        }
    }
} 