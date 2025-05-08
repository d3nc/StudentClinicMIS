using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int? MedicalRecordId { get; set; }

    public string Type { get; set; } = null!;

    public string? Details { get; set; }

    public string? Status { get; set; }

    public int? MedicineId { get; set; }

    public int? AppointmentId { get; set; }

    public DateOnly? PrescriptionDate { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public virtual Medicine? Medicine { get; set; }

    public virtual ICollection<PrescribedMedicine> PrescribedMedicines { get; set; } = new List<PrescribedMedicine>();
}
