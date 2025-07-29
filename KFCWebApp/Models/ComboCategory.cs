using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class ComboCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục combo")]
        [StringLength(100, ErrorMessage = "Tên danh mục combo không được vượt quá 100 ký tự")]
        [Display(Name = "Tên danh mục combo")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Combo> Combos { get; set; } = new List<Combo>();
    }
} 