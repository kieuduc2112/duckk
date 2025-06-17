using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFCWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        public bool IsAvailable { get; set; } = true;

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
} 