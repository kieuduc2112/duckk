using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsAvailable { get; set; }

    public int CategoryId { get; set; }

    public int Size { get; set; }

    public int? PieceCount { get; set; }

    public int OptionType { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<OrderDetail> OrderDetailProductId1Navigations { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderDetail> OrderDetailProducts { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductPromotion> ProductPromotionProduct1s { get; set; } = new List<ProductPromotion>();

    public virtual ICollection<ProductPromotion> ProductPromotionProduct2s { get; set; } = new List<ProductPromotion>();
}
