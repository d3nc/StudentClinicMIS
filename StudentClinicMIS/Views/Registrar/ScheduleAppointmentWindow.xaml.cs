using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudentClinicMIS.Views
{
    public partial class ScheduleAppointmentWindow : Window
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly PolyclinicContext _context;
        private readonly int _patientId;

        private List<Department> _departments = new();
        private List<Doctor> _doctors = new();
        private List<Appointment> _appointments = new();
        private const int MaxAppointmentsPerDay = 10;

        public ScheduleAppointmentWindow(
            int patientId,
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            PolyclinicContext context)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _context = context;
            _patientId = patientId;

            _ = LoadDepartmentsAsync();
        }

        private async Task LoadDepartmentsAsync()
        {
            _departments = await Task.Run(() =>
                _context.Departments.OrderBy(d => d.Name).ToList());
            DepartmentComboBox.ItemsSource = _departments;
        }

        private async void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoctorComboBox.ItemsSource = null;
            if (DepartmentComboBox.SelectedItem is Department selectedDepartment)
            {
                _doctors = await _doctorRepository.GetByDepartmentIdAsync(selectedDepartment.DepartmentId);
                DoctorComboBox.ItemsSource = _doctors.Select(d => new
                {
                    d.DoctorId,
                    FullName = $"{d.Employee.LastName} {d.Employee.FirstName} {d.Employee.MiddleName}"
                }).ToList();
            }
        }

        private async void DoctorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateTimeSlotsAsync();
            await UpdateCalendarVisualsAsync();
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateTimeSlotsAsync();
        }

        private async Task UpdateTimeSlotsAsync()
        {
            TimeSlotsListBox.ItemsSource = null;

            if (DoctorComboBox.SelectedValue == null ||
                DatePicker.SelectedDate == null)
                return;

            int doctorId = (int)DoctorComboBox.SelectedValue;
            DateOnly selectedDate = DateOnly.FromDateTime(DatePicker.SelectedDate.Value);

            _appointments = await _appointmentRepository
                .GetAppointmentsByDoctorAndDateAsync(doctorId, selectedDate);

            var slots = GenerateTimeSlots("09:00", "15:00", TimeSpan.FromMinutes(30));

            var slotItems = slots.Select(slot => new TimeSlotDisplay
            {
                Time = slot,
                Display = slot.ToString("HH:mm"),
                IsOccupied = _appointments.Any(a => a.StartTime == slot)
            }).ToList();

            TimeSlotsListBox.ItemsSource = slotItems;
        }

        private List<TimeOnly> GenerateTimeSlots(string start, string end, TimeSpan step)
        {
            var slots = new List<TimeOnly>();
            var startTime = TimeOnly.ParseExact(start, "HH:mm", CultureInfo.InvariantCulture);
            var endTime = TimeOnly.ParseExact(end, "HH:mm", CultureInfo.InvariantCulture);

            while (startTime < endTime)
            {
                slots.Add(startTime);
                startTime = startTime.Add(step);
            }

            return slots;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorComboBox.SelectedValue == null ||
                DatePicker.SelectedDate == null ||
                TimeSlotsListBox.SelectedItem is not TimeSlotDisplay selectedSlot ||
                selectedSlot.IsOccupied)
            {
                MessageBox.Show("Пожалуйста, выберите доступный слот для приёма.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int doctorId = (int)DoctorComboBox.SelectedValue;

            var alreadyExists = _appointments.Any(a => a.StartTime == selectedSlot.Time);

            if (alreadyExists)
            {
                MessageBox.Show("Выбранный слот уже занят. Обновите список или выберите другое время.",
                                "Слот занят", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var appointment = new Appointment
            {
                PatientId = _patientId,
                DoctorId = doctorId,
                AppointmentDate = DateOnly.FromDateTime(DatePicker.SelectedDate.Value),
                StartTime = selectedSlot.Time,
                Status = "Запланирован",
                CreatedAt = DateTime.Now
            };

            await _appointmentRepository.AddAsync(appointment);
            MessageBox.Show("Запись успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private async Task UpdateCalendarVisualsAsync()
        {
            if (DoctorComboBox.SelectedValue is not int doctorId)
                return;

            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(30);
            var busyDates = new HashSet<DateOnly>();
            var freeDates = new HashSet<DateOnly>();

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);
                if (appointments.Count >= MaxAppointmentsPerDay)
                    busyDates.Add(date);
                else if (appointments.Count > 0)
                    freeDates.Add(date);
            }

            if (Resources["DateToSlotStatusConverter"] is Converters.DateToSlotStatusConverter converter)
            {
                converter.BusyDates = busyDates;
                converter.FreeDates = freeDates;
            }

            DatePicker.DisplayDateStart = DateTime.Today;
            DatePicker.DisplayDateEnd = DateTime.Today.AddDays(30);
        }

        private class TimeSlotDisplay
        {
            public TimeOnly Time { get; set; }
            public string Display { get; set; }
            public bool IsOccupied { get; set; }
        }
    }
}
