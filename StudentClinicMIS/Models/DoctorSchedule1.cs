using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class DoctorSchedule1
{
    public int ScheduleId { get; set; }

    public int DoctorId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int? RoomId { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Room? Room { get; set; }
}
