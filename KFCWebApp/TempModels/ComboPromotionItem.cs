using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class ComboPromotionItem
{
    public int Id { get; set; }

    public int ComboPromotionId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual ComboPromotion ComboPromotion { get; set; } = null!;
}
