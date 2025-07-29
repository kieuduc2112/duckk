using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public string ItemType { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int? ProductId1 { get; set; }

    public int? ComboId { get; set; }

    public int? ComboId1 { get; set; }

    public string ItemName { get; set; } = null!;

    public virtual Combo? Combo { get; set; }

    public virtual Combo? ComboId1Navigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual Product? ProductId1Navigation { get; set; }
}
