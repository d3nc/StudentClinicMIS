using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    public class MedicalClearance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClearanceId { get; set; }

        [ForeignKey("Patient")]
        public int? PatientId { get; set; }

        [ForeignKey("Doctor")]
        public int? DoctorId { get; set; }

        public DateOnly? ExaminationDate { get; set; }
        public DateOnly? ValidUntil { get; set; }
        public string Conclusion { get; set; }
        public string Restrictions { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}