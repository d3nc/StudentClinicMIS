using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.ViewModels.Doctor
{
    public class SchedulePageViewModel : BaseViewModel
    {
        private readonly PolyclinicContext _context;
        private readonly int _doctorId;

        public ObservableCollection<Appointment> FilteredAppointments { get; set; } = new();
        public ObservableCollection<string> StatusOptions { get; } = new() { "Все", "Запланирован", "Завершён", "Неявка" };

        public DateTime? SelectedDate { get; set; }
        public string SelectedStatus { get; set; } = "Все";

        public ICommand FilterCommand { get; }
        public ICommand MarkAsCompletedCommand { get; }
        public ICommand MarkAsNoShowCommand { get; }

        public SchedulePageViewModel(PolyclinicContext context, int doctorId)
        {
            _context = context;
            _doctorId = doctorId;

            FilterCommand = new RelayCommand(ApplyFilter);
            MarkAsCompletedCommand = new RelayCommand<Appointment>(appt => ChangeStatus(appt, "Завершён"));
            MarkAsNoShowCommand = new RelayCommand<Appointment>(appt => ChangeStatus(appt, "Неявка"));

            LoadAppointments();
        }

        private void LoadAppointments()
        {
            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == _doctorId)
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            FilteredAppointments.Clear();
            foreach (var appt in appointments)
                FilteredAppointments.Add(appt);
        }

        private void ApplyFilter()
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == _doctorId);

            if (SelectedDate.HasValue)
                query = query.Where(a => a.AppointmentDate == DateOnly.FromDateTime(SelectedDate.Value));

            if (!string.IsNullOrWhiteSpace(SelectedStatus) && SelectedStatus != "Все")
                query = query.Where(a => a.Status == SelectedStatus);

            var result = query
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            FilteredAppointments.Clear();
            foreach (var appt in result)
                FilteredAppointments.Add(appt);
        }

        private void ChangeStatus(Appointment? appointment, string status)
        {
            if (appointment == null) return;

            appointment.Status = status;
            _context.SaveChanges();
            ApplyFilter();
        }
    }
}
