using System;

namespace StudentClinicMIS.ViewModels.Registrar
{
    public class DoctorScheduleSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Display { get; set; } = string.Empty;

        public override string ToString() => Display;
    }
}
