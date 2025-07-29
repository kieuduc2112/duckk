using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class ComboCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Combo> Combos { get; set; } = new List<Combo>();
}
