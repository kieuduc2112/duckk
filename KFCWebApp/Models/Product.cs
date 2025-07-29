using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFCWebApp.Models
{
    public enum ProductSize
    {
        [Display(Name = "Nhỏ")]
        Small = 1,
        [Display(Name = "Vừa")]
        Medium = 2,
        [Display(Name = "Lớn")]
        Large = 3,
        [Display(Name = "Không áp dụng")]
        None = 0
    }

    public enum ProductOptionType
    {
        Size = 1,
        Piece = 2
    }

    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Kích thước")]
        public ProductSize Size { get; set; } = ProductSize.None;

        [Display(Name = "Kiểu thuộc tính")]
        public ProductOptionType OptionType { get; set; } = ProductOptionType.Size;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Còn bán")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Số lượng miếng gà")]
        public int? PieceCount { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
} 