using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class ComboPromotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public string RewardType { get; set; } = null!;

    public decimal? RewardValue { get; set; }

    public int? RewardProductId { get; set; }

    public virtual ICollection<ComboPromotionItem> ComboPromotionItems { get; set; } = new List<ComboPromotionItem>();
}
