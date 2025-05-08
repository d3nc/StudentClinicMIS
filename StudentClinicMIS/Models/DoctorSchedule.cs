using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class DoctorSchedule
{
    public string? Doctor { get; set; }

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Room { get; set; }

    public string? Department { get; set; }
}
