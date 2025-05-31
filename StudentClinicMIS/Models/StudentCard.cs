using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    [Table("student_cards")]
    public class StudentCard
    {
        [Key]
        [Column("card_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardId { get; set; }

        [Column("patient_id")]
        public int PatientId { get; set; }

        [Column("group_id")]
        [ForeignKey("Group")]
        public int? GroupId { get; set; }

        [Column("card_number")]
        [Required]
        [StringLength(20)]
        public string CardNumber { get; set; }

        [Column("issue_date")]
        public DateOnly? IssueDate { get; set; }

        [Column("expiry_date")]
        public DateOnly? ExpiryDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public virtual Patient Patient { get; set; }
        public virtual StudentGroup Group { get; set; }
    }
}