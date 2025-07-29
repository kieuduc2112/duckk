using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Email { get; set; }

    public string? Note { get; set; }

    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ApplicationUserId { get; set; }

    public virtual AspNetUser? ApplicationUser { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
