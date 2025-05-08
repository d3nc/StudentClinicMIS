using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Gender
{
    public int GenderId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
