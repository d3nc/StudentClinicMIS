using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public string? InternalPhone { get; set; }

    public int? HeadDoctorId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Doctor? HeadDoctor { get; set; }

    public virtual ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
