using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class DepartmentStat
{
    public string? Department { get; set; }

    public long? DoctorsCount { get; set; }

    public long? AppointmentsCount { get; set; }

    public long? UniquePatientsCount { get; set; }

    public string? Location { get; set; }

    public string? InternalPhone { get; set; }

    public string? HeadDoctor { get; set; }
}
