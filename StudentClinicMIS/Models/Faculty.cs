using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClinicMIS.Models
{
    [Table("faculties")]
    public class Faculty
    {
        [Key]
        [Column("faculty_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FacultyId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("short_name")]
        [StringLength(20)]
        public string ShortName { get; set; }

        [Column("dean")]
        [StringLength(100)]
        public string Dean { get; set; }

        public virtual ICollection<StudentGroup> Groups { get; set; } = new List<StudentGroup>();
    }
}