using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    [Table("student_groups")]
    public class StudentGroup
    {
        [Key]
        [Column("group_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("faculty_id")]
        [ForeignKey("Faculty")]
        public int? FacultyId { get; set; }

        [Column("course_number")]
        public int? CourseNumber { get; set; }

        [Column("student_specialization_id")]
        public int? StudentSpecializationId { get; set; }

        [Column("education_form")]
        public string EducationForm { get; set; }

        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<StudentCard> StudentCards { get; set; } = new List<StudentCard>();
    }
}