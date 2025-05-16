using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? InsuranceNumber { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public int? UserId { get; set; }

    public int? GenderId { get; set; }

    public int? InsuranceCompanyId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Gender? Gender { get; set; }

    public virtual InsuranceCompany? InsuranceCompany { get; set; }

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual User? User { get; set; }
    [NotMapped]
    public string GenderName => Gender?.Name ?? "Не указан";

    // Вычисляемое свойство для форматирования даты
    [NotMapped]
    public string FormattedBirthDate => BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана";
}
