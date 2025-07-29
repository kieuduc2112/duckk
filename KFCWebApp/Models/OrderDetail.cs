using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        // Sản phẩm hoặc combo
        public int? ProductId { get; set; }
        public int? ComboId { get; set; }

        [Required]
        [Display(Name = "Tên sản phẩm/Combo")]
        public string ItemName { get; set; } = string.Empty;

        [Display(Name = "Loại")]
        public string ItemType { get; set; } = "Product"; // "Product" hoặc "Combo"

        [Display(Name = "Giá")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Price { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Total => Price * Quantity;

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [ForeignKey("ComboId")]
        public Combo? Combo { get; set; }
    }
} 