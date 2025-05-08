using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int EmployeeId { get; set; }

    public int? SpecializationId { get; set; }

    public string? Qualification { get; set; }

    public string? LicenseNumber { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<DoctorSchedule1> DoctorSchedule1s { get; set; } = new List<DoctorSchedule1>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual Specialization? Specialization { get; set; }
}
