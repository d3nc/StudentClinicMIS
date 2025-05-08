using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class PatientAppointment
{
    public string? Patient { get; set; }

    public DateOnly? AppointmentDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Status { get; set; }

    public string? Purpose { get; set; }

    public string? Doctor { get; set; }

    public string? Specialization { get; set; }

    public string? Room { get; set; }
}
