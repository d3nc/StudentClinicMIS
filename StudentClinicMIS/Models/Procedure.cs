using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Procedure
{
    public int ProcedureId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Duration { get; set; }

    public int? DepartmentId { get; set; }

    public decimal? Price { get; set; }

    public virtual Department? Department { get; set; }
}
