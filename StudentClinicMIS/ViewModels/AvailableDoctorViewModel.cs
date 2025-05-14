using StudentClinicMIS.Models;
using System.Collections.Generic;

namespace StudentClinicMIS.ViewModels
{
    public class AvailableDoctorViewModel
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;

        public List<Appointment> Appointments { get; set; } = new();
    }
}
