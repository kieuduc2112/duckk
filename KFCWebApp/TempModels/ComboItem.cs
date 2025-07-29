using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class ComboItem
{
    public int Id { get; set; }

    public int ComboId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public bool IsExchangeable { get; set; }

    public bool IsRequired { get; set; }

    public virtual Combo Combo { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
