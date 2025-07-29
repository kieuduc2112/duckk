using System;
using System.Collections.Generic;

namespace KFCWebApp.TempModels;

public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public string ConditionType { get; set; } = null!;

    public string ConditionJson { get; set; } = null!;

    public string RewardJson { get; set; } = null!;
}
