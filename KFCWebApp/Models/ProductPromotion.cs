using System;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class ProductPromotion
    {
        public int Id { get; set; }
        [Required]
        public int ProductId1 { get; set; }
        public Product Product1 { get; set; }
        [Required]
        public int ProductId2 { get; set; }
        public Product? Product2 { get; set; }
        public decimal? PercentDiscount { get; set; }
        public decimal? AmountDiscount { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
        public bool IsActive { get; set; } = true;
    }
} 