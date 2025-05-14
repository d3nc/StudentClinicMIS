using System;
using System.Collections.Generic;

namespace StudentClinicMIS.ViewModels
{
    public class DoctorWithSlotsViewModel
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public List<TimeOnly> FreeSlots { get; set; }
    }
}
