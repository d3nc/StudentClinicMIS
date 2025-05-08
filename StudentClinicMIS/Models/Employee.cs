using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public DateOnly? HireDate { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? DepartmentId { get; set; }

    public int? UserId { get; set; }

    public int? GenderId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Gender? Gender { get; set; }

    public virtual User? User { get; set; }
}
