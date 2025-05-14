using StudentClinicMIS.Models;
using System;
using System.Collections.ObjectModel;

namespace StudentClinicMIS.ViewModels.Registrar
{
    public class DoctorSlotViewModel
    {
        public string FullName => $"{Doctor.Employee?.LastName} {Doctor.Employee?.FirstName} {Doctor.Employee?.MiddleName}";
        public string Specialization => Doctor.Specialization?.Name ?? "";
        public ObservableCollection<DoctorScheduleSlot> FreeSlots { get; } = new();

        public Doctor Doctor { get; }

        public DoctorSlotViewModel(Doctor doctor, DoctorScheduleEntity schedule, DateOnly appointmentDate)
        {
            Doctor = doctor;

            var dateTime = appointmentDate.ToDateTime(schedule.StartTime);
            var endDateTime = appointmentDate.ToDateTime(schedule.EndTime);

            while (dateTime < endDateTime)
            {
                var endTime = dateTime.AddMinutes(30);

                FreeSlots.Add(new DoctorScheduleSlot
                {
                    StartTime = dateTime.TimeOfDay, // TimeSpan
                    EndTime = endTime.TimeOfDay,    // TimeSpan
                    Display = $"{dateTime:HH\\:mm}-{endTime:HH\\:mm}"
                });

                dateTime = endTime;
            }
        }
    }
}
