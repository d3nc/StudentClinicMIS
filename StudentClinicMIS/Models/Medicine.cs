using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string Name { get; set; } = null!;

    public string? Form { get; set; }

    public string? Dosage { get; set; }

    public string? Manufacturer { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public int? MinimumStock { get; set; }

    public virtual ICollection<PrescribedMedicine> PrescribedMedicines { get; set; } = new List<PrescribedMedicine>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
