using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentClinicMIS.Models;

public partial class PolyclinicContext : DbContext
{
    public PolyclinicContext()
    {
    }

    public PolyclinicContext(DbContextOptions<PolyclinicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentStat> DepartmentStats { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<DoctorScheduleEntity> DoctorSchedules1 { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<InsuranceCompany> InsuranceCompanies { get; set; }

    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientAppointment> PatientAppointments { get; set; }

    public virtual DbSet<PrescribedMedicine> PrescribedMedicines { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Procedure> Procedures { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=polyclinic;Username=user;Password=password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("ru_RU.UTF-8");

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Purpose)
                .HasMaxLength(255)
                .HasColumnName("purpose");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("appointments_doctor_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("appointments_patient_id_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.HeadDoctorId).HasColumnName("head_doctor_id");
            entity.Property(e => e.InternalPhone)
                .HasMaxLength(20)
                .HasColumnName("internal_phone");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.HeadDoctor).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HeadDoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_departments_head");
        });

        modelBuilder.Entity<DepartmentStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("department_stats");

            entity.Property(e => e.AppointmentsCount).HasColumnName("appointments_count");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.DoctorsCount).HasColumnName("doctors_count");
            entity.Property(e => e.HeadDoctor).HasColumnName("head_doctor");
            entity.Property(e => e.InternalPhone)
                .HasMaxLength(20)
                .HasColumnName("internal_phone");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.UniquePatientsCount).HasColumnName("unique_patients_count");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("doctors_pkey");

            entity.ToTable("doctors");

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(50)
                .HasColumnName("license_number");
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .HasColumnName("qualification");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("doctors_employee_id_fkey");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("doctors_specialization_id_fkey");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("doctor_schedule");

            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(15)
                .HasColumnName("day_of_week");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.Doctor).HasColumnName("doctor");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Room)
                .HasMaxLength(20)
                .HasColumnName("room");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<DoctorScheduleEntity>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("doctor_schedules_pkey");

            entity.ToTable("doctor_schedules");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(15)
                .HasColumnName("day_of_week");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedule1s)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("doctor_schedules_doctor_id_fkey");

            entity.HasOne(d => d.Room).WithMany(p => p.DoctorSchedule1s)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("doctor_schedules_room_id_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("employees_department_id_fkey");

            entity.HasOne(d => d.Gender).WithMany(p => p.Employees)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("employees_gender_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Employees)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("employees_user_id_fkey");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("genders_pkey");

            entity.ToTable("genders");

            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<InsuranceCompany>(entity =>
        {
            entity.HasKey(e => e.InsuranceCompanyId).HasName("insurance_companies_pkey");

            entity.ToTable("insurance_companies");

            entity.Property(e => e.InsuranceCompanyId).HasColumnName("insurance_company_id");
            entity.Property(e => e.ContractNumber)
                .HasMaxLength(50)
                .HasColumnName("contract_number");
            entity.Property(e => e.CooperationEndDate).HasColumnName("cooperation_end_date");
            entity.Property(e => e.CooperationStartDate).HasColumnName("cooperation_start_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LegalAddress).HasColumnName("legal_address");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ShortName)
                .HasMaxLength(50)
                .HasColumnName("short_name");
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .HasColumnName("website");
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("medical_history");

            entity.Property(e => e.Diagnosis).HasColumnName("diagnosis");
            entity.Property(e => e.Doctor).HasColumnName("doctor");
            entity.Property(e => e.Patient).HasColumnName("patient");
            entity.Property(e => e.RecordDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("record_date");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .HasColumnName("specialization");
            entity.Property(e => e.Symptoms).HasColumnName("symptoms");
            entity.Property(e => e.Treatment).HasColumnName("treatment");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("medical_records_pkey");

            entity.ToTable("medical_records");

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.Diagnosis).HasColumnName("diagnosis");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.RecordDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("record_date");
            entity.Property(e => e.Symptoms).HasColumnName("symptoms");
            entity.Property(e => e.Treatment).HasColumnName("treatment");

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("medical_records_doctor_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("medical_records_patient_id_fkey");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId).HasName("medicines_pkey");

            entity.ToTable("medicines");

            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .HasColumnName("dosage");
            entity.Property(e => e.Form)
                .HasMaxLength(50)
                .HasColumnName("form");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(100)
                .HasColumnName("manufacturer");
            entity.Property(e => e.MinimumStock).HasColumnName("minimum_stock");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("patients_pkey");

            entity.ToTable("patients");

            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.InsuranceCompanyId).HasColumnName("insurance_company_id");
            entity.Property(e => e.InsuranceNumber)
                .HasMaxLength(50)
                .HasColumnName("insurance_number");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RegistrationDate).HasColumnName("registration_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Gender).WithMany(p => p.Patients)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("patients_gender_id_fkey");

            entity.HasOne(d => d.InsuranceCompany).WithMany(p => p.Patients)
                .HasForeignKey(d => d.InsuranceCompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("patients_insurance_company_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Patients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("patients_user_id_fkey");
        });

        modelBuilder.Entity<PatientAppointment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("patient_appointments");

            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.Doctor).HasColumnName("doctor");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Patient).HasColumnName("patient");
            entity.Property(e => e.Purpose)
                .HasMaxLength(255)
                .HasColumnName("purpose");
            entity.Property(e => e.Room)
                .HasMaxLength(20)
                .HasColumnName("room");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .HasColumnName("specialization");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<PrescribedMedicine>(entity =>
        {
            entity.HasKey(e => e.PrescribedMedicineId).HasName("prescribed_medicines_pkey");

            entity.ToTable("prescribed_medicines");

            entity.Property(e => e.PrescribedMedicineId).HasColumnName("prescribed_medicine_id");
            entity.Property(e => e.Duration)
                .HasMaxLength(50)
                .HasColumnName("duration");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.PrescriptionDate).HasColumnName("prescription_date");
            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UsageInstructions).HasColumnName("usage_instructions");

            entity.HasOne(d => d.Medicine).WithMany(p => p.PrescribedMedicines)
                .HasForeignKey(d => d.MedicineId)
                .HasConstraintName("prescribed_medicines_medicine_id_fkey");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescribedMedicines)
                .HasForeignKey(d => d.PrescriptionId)
                .HasConstraintName("prescribed_medicines_prescription_id_fkey");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("prescriptions_pkey");

            entity.ToTable("prescriptions");

            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.MedicalRecordId).HasColumnName("medical_record_id");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.PrescriptionDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("prescription_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("prescriptions_appointment_id_fkey");

            entity.HasOne(d => d.MedicalRecord).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("prescriptions_medical_record_id_fkey");

            entity.HasOne(d => d.Medicine).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("prescriptions_medicine_id_fkey");
        });

        modelBuilder.Entity<Procedure>(entity =>
        {
            entity.HasKey(e => e.ProcedureId).HasName("procedures_pkey");

            entity.ToTable("procedures");

            entity.Property(e => e.ProcedureId).HasColumnName("procedure_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration)
                .HasMaxLength(50)
                .HasColumnName("duration");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.Department).WithMany(p => p.Procedures)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("procedures_department_id_fkey");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("rooms_pkey");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Equipment).HasColumnName("equipment");
            entity.Property(e => e.Floor).HasColumnName("floor");
            entity.Property(e => e.Number)
                .HasMaxLength(20)
                .HasColumnName("number");

            entity.HasOne(d => d.Department).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("rooms_department_id_fkey");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.SpecializationId).HasName("specializations_pkey");

            entity.ToTable("specializations");

            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("tests_pkey");

            entity.ToTable("tests");

            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ExecutionTime)
                .HasMaxLength(50)
                .HasColumnName("execution_time");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Preparation).HasColumnName("preparation");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.Department).WithMany(p => p.Tests)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tests_department_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });
        modelBuilder.HasSequence("nonъзователи_id_nользователя_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
