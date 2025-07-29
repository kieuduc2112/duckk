using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class ProductPromotion
{
    public int Id { get; set; }

    public int ProductId2 { get; set; }

    public decimal? PercentDiscount { get; set; }

    public decimal? AmountDiscount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public int Product1Id { get; set; }

    public int Product2Id { get; set; }

    public int ProductId1 { get; set; }

    public virtual Product Product1 { get; set; } = null!;

    public virtual Product Product2 { get; set; } = null!;
}
