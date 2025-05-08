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

    public virtual ICollection<DoctorSchedule1> DoctorSchedule1s { get; set; } = new List<DoctorSchedule1>();
}
