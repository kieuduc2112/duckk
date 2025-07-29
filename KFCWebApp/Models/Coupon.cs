using System;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public enum CouponType
    {
        Percentage, // Giảm theo %
        Amount      // Giảm theo số tiền
    }

    public class Coupon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(50)]
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Loại giảm giá")]
        public CouponType Type { get; set; }

        [Required]
        [Display(Name = "Giá trị")]
        public decimal Value { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

        [Display(Name = "Số lượng còn lại")]
        public int Quantity { get; set; } = 0;

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Danh mục")]
        public int? CouponCategoryId { get; set; }
        public CouponCategory? CouponCategory { get; set; }
    }
} 