namespace StudentClinicMIS.Models
{
    public class Vaccination
    {
        public int VaccinationId { get; set; }
        public int? PatientId { get; set; }
        public string VaccineName { get; set; }
        public DateOnly? VaccinationDate { get; set; }
        public DateOnly? NextVaccinationDate { get; set; }
        public int? DoctorId { get; set; }
        public string Notes { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}