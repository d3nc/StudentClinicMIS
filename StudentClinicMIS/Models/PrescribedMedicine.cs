using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class PrescribedMedicine
{
    public int PrescribedMedicineId { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }

    public string? UsageInstructions { get; set; }

    public string? Duration { get; set; }

    public DateOnly? PrescriptionDate { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
