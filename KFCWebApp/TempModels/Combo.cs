using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class Combo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    public int ComboCategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsFeatured { get; set; }

    public virtual ComboCategory ComboCategory { get; set; } = null!;

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<OrderDetail> OrderDetailComboId1Navigations { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderDetail> OrderDetailCombos { get; set; } = new List<OrderDetail>();
}
