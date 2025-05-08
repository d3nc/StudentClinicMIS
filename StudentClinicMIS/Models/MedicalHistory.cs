using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class MedicalHistory
{
    public string? Patient { get; set; }

    public DateTime? RecordDate { get; set; }

    public string? Symptoms { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Doctor { get; set; }

    public string? Specialization { get; set; }
}
