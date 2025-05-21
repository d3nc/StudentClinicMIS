using StudentClinicMIS.Models;
using System;
using System.Collections.ObjectModel;

namespace StudentClinicMIS.ViewModels.Registrar
{
    public class DoctorSlotViewModel
    {
        public string FullName => $"{Doctor.Employee?.LastName} {Doctor.Employee?.FirstName} {Doctor.Employee?.MiddleName}";
        public string Specialization => Doctor.Specialization?.Name ?? "";
        public ObservableCollection<ScheduleSlot> FreeSlots { get; } = new();

        public Models.Doctor Doctor { get; }

        public DoctorSlotViewModel(Models.Doctor doctor, DoctorScheduleEntity schedule, DateOnly appointmentDate)
        {
            Doctor = doctor;

            var dateTime = appointmentDate.ToDateTime(schedule.StartTime);
            var endDateTime = appointmentDate.ToDateTime(schedule.EndTime);

            while (dateTime < endDateTime)
            {
                var endTime = dateTime.AddMinutes(30);

                FreeSlots.Add(new ScheduleSlot
                {
                    Start = dateTime.TimeOfDay,
                    End = endTime.TimeOfDay,
                    SlotDisplay = $"{dateTime:HH\\:mm}-{endTime:HH\\:mm}"
                });

                dateTime = endTime;
            }
        }
    }

    public class ScheduleSlot
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string SlotDisplay { get; set; }
    }
}