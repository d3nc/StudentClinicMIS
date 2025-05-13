using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string Number { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public int? Floor { get; set; }

    public string? Equipment { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<DoctorScheduleEntity> DoctorSchedule1s { get; set; } = new List<DoctorScheduleEntity>();
}
