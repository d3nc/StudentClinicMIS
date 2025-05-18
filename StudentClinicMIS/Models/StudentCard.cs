using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    [Table("student_cards")]
    public class StudentCard
    {
        [Key]
        [Column("card_id")]  // Указываем точное имя столбца
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardId { get; set; }

        [Column("patient_id")]  // Указываем точное имя столбца
        public int PatientId { get; set; }

        [Column("group_id")]  // Указываем точное имя столбца
        [ForeignKey("Group")]
        public int? GroupId { get; set; }

        [Column("card_number")]  // Указываем точное имя столбца
        [Required]
        [StringLength(20)]
        public string CardNumber { get; set; }

        [Column("issue_date")]
        public DateOnly? IssueDate { get; set; }

        [Column("expiry_date")]
        public DateOnly? ExpiryDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual Patient Patient { get; set; }
        public virtual StudentGroup Group { get; set; }
    }
}