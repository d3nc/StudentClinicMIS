using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Test
{
    public int TestId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Preparation { get; set; }

    public string? ExecutionTime { get; set; }

    public int? DepartmentId { get; set; }

    public decimal? Price { get; set; }

    public virtual Department? Department { get; set; }
}
