using System;
using System.Collections.Generic;

namespace StudentClinicMIS.Models;

public partial class InsuranceCompany
{
    public int InsuranceCompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? LegalAddress { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? ContractNumber { get; set; }

    public DateOnly? CooperationStartDate { get; set; }

    public DateOnly? CooperationEndDate { get; set; }

    public bool? IsActive { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
