using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFCWebApp.Models
{
    public class ComboItem
    {
        public int Id { get; set; }

        [Required]
        public int ComboId { get; set; }
        public Combo? Combo { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Có thể thay đổi")]
        public bool IsExchangeable { get; set; } = false;

        [Display(Name = "Bắt buộc")]
        public bool IsRequired { get; set; } = true;
    }
} 