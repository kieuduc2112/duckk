using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class Coupon
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public int Type { get; set; }

    public decimal Value { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int Quantity { get; set; }

    public bool IsActive { get; set; }

    public int? CouponCategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CouponCategory? CouponCategory { get; set; }
}
