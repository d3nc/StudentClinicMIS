using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StudentClinicMIS.Views
{
    public partial class AddAppointmentWindow : Window
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly int _patientId;
        private List<DoctorViewModel> _doctorViewModels = new();

        public AddAppointmentWindow(int patientId, IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _patientId = patientId;

            _ = LoadDoctorsAsync();
        }

        private async Task LoadDoctorsAsync()
        {
            var doctors = await _appointmentRepository.GetAllDoctorsAsync();

            _doctorViewModels = doctors
                .Where(d => d.Employee != null)
                .Select(d => new DoctorViewModel
                {
                    DoctorId = d.DoctorId,
                    FullName = $"{d.Employee.LastName} {d.Employee.FirstName} {d.Employee.MiddleName}"
                })
                .ToList();

            DoctorComboBox.ItemsSource = _doctorViewModels;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorComboBox.SelectedItem is not DoctorViewModel selectedDoctor)
            {
                MessageBox.Show("Выберите врача.");
                return;
            }

            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату.");
                return;
            }

            if (!TimeOnly.TryParseExact(StartTimeBox.Text.Trim(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startTime))
            {
                MessageBox.Show("Неверный формат времени. Пример: 09:00");
                return;
            }

            var appointment = new Appointment
            {
                PatientId = _patientId,
                DoctorId = selectedDoctor.DoctorId,
                AppointmentDate = DateOnly.FromDateTime(DatePicker.SelectedDate.Value),
                StartTime = startTime,
                Status = "Запланирован",
                CreatedAt = DateTime.Now
            };

            await _appointmentRepository.AddAsync(appointment);
            DialogResult = true;
            Close();
        }
    }
}
