using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100)]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 