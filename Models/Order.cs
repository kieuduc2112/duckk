using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFCWebApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$", 
            ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Note { get; set; }

        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Display(Name = "Ngày đặt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum OrderStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending,
        [Display(Name = "Đã xác nhận")]
        Confirmed,
        [Display(Name = "Đang xử lý")]
        Processing,
        [Display(Name = "Đang giao hàng")]
        Shipping,
        [Display(Name = "Đã giao hàng")]
        Delivered,
        [Display(Name = "Đã hoàn thành")]
        Completed,
        [Display(Name = "Đã hủy")]
        Cancelled
    }
} 