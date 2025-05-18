using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    [Table("faculties")] // Явно указываем имя таблицы
    public class Faculty
    {
        [Key]
        [Column("faculty_id")] // Точное имя столбца
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FacultyId { get; set; }

        [Column("name")] // Точное имя столбца
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("short_name")] // Точное имя столбца
        [StringLength(20)]
        public string ShortName { get; set; }

        [Column("dean")] // Точное имя столбца
        [StringLength(100)]
        public string Dean { get; set; }

        // Навигационное свойство
        public virtual ICollection<StudentGroup> Groups { get; set; } = new List<StudentGroup>();
    }
}