using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFCWebApp.Models
{
    public class Combo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên combo")]
        [StringLength(200, ErrorMessage = "Tên combo không được vượt quá 200 ký tự")]
        [Display(Name = "Tên combo")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá combo")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá combo phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá combo")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Giá gốc")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục combo")]
        [Display(Name = "Danh mục combo")]
        public int ComboCategoryId { get; set; }
        public ComboCategory? ComboCategory { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự")]
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Còn bán")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Combo nổi bật")]
        public bool IsFeatured { get; set; } = false;

        public ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
} 