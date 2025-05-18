using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    [Table("student_groups")] // Явно указываем имя таблицы в snake_case
    public class StudentGroup
    {
        [Key]
        [Column("group_id")] // Указываем точное имя столбца
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        [Column("name")] // Указываем точное имя столбца
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("faculty_id")] // Указываем точное имя столбца
        [ForeignKey("Faculty")]
        public int? FacultyId { get; set; }

        [Column("course_number")] // Указываем точное имя столбца
        public int? CourseNumber { get; set; }

        [Column("student_specialization_id")] // Добавляем отсутствующий столбец
        public int? StudentSpecializationId { get; set; }

        [Column("education_form")] // Указываем точное имя столбца, если он существует
        public string EducationForm { get; set; }

        // Навигационные свойства
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<StudentCard> StudentCards { get; set; } = new List<StudentCard>();
    }
}