using System;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
        public bool IsActive { get; set; } = true;

        // Loại điều kiện: BuyXGetY, PercentDiscount, AmountDiscount, ...
        [Required]
        [StringLength(50)]
        public string ConditionType { get; set; } = string.Empty;

        // Điều kiện áp dụng (dạng JSON)
        [Required]
        public string ConditionJson { get; set; } = string.Empty;

        // Phần thưởng (dạng JSON)
        [Required]
        public string RewardJson { get; set; } = string.Empty;
    }
} 