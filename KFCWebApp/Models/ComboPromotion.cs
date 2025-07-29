using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class ComboPromotion
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
        public List<ComboPromotionItem> Items { get; set; } = new();
        // Phần thưởng: RewardType = Percent, Amount, Gift
        [StringLength(20)]
        public string RewardType { get; set; } = "Percent";
        public decimal? RewardValue { get; set; }
        public int? RewardProductId { get; set; }
    }
} 