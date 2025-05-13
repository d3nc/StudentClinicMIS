using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClinicMIS.ViewModels
{
    public class DoctorWithSlotsViewModel
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public List<TimeOnly> FreeSlots { get; set; } = new();
    }
}
