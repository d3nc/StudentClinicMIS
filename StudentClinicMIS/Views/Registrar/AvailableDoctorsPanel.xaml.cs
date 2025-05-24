using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace StudentClinicMIS.Views.Registrar
{
    public partial class AvailableDoctorsPanel : UserControl
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private ObservableCollection<Department> _departments = new();
        private ObservableCollection<Doctor> _doctors = new();
        private ObservableCollection<AppointmentSlot> _slots = new();

        public Patient? CurrentPatient { get; set; }

        public AvailableDoctorsPanel()
        {
            InitializeComponent();
            _scopeFactory = (IServiceScopeFactory)App.AppHost.Services.GetService(typeof(IServiceScopeFactory))!;
            LoadDepartmentsAsync();
        }

        public void UpdatePatientInfo()
        {
            if (CurrentPatient != null)
            {
                PatientTextBlock.Text = $"Пациент: {CurrentPatient.LastName} {CurrentPatient.FirstName} {CurrentPatient.MiddleName}";
            }
            else
            {
                PatientTextBlock.Text = "Пациент не выбран";
            }
        }

        private async void LoadDepartmentsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var departmentRepository = scope.ServiceProvider.GetRequiredService<IDepartmentRepository>();

            var departments = await departmentRepository.GetAllAsync();
            _departments = new ObservableCollection<Department>(departments);
            DepartmentComboBox.ItemsSource = _departments;
        }

        private async void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmentComboBox.SelectedItem is not Department department)
                return;

            using var scope = _scopeFactory.CreateScope();
            var doctorRepository = scope.ServiceProvider.GetRequiredService<IDoctorRepository>();

            var doctors = await doctorRepository.GetByDepartmentIdAsync(department.DepartmentId);
            _doctors = new ObservableCollection<Doctor>(doctors);
            DoctorComboBox.ItemsSource = _doctors;
        }

        private async void AppointmentDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadSlotsAsync();
        }

        private async void DoctorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadSlotsAsync();
        }

        private async void SlotButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TimeOnly timeSlot)
            {
                MessageBox.Show($"Выбран слот: {timeSlot:hh\\:mm}", "Выбор времени",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task LoadSlotsAsync()
        {
            _slots.Clear();

            if (DoctorComboBox.SelectedItem is not Doctor doctor ||
                AppointmentDatePicker.SelectedDate is not DateTime selectedDate)
                return;

            var dateOnly = DateOnly.FromDateTime(selectedDate);

            using var scope = _scopeFactory.CreateScope();
            var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
            var doctorRepository = scope.ServiceProvider.GetRequiredService<IDoctorRepository>();

            var appointments = await appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctor.DoctorId, dateOnly);
            var schedule = await doctorRepository.GetScheduleForDoctorAsync(doctor.DoctorId, dateOnly.DayOfWeek);

            if (schedule is null)
            {
                MessageBox.Show("У врача нет расписания на выбранный день.", "Нет расписания", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var start = schedule.StartTime;
            var end = schedule.EndTime;
            var interval = TimeSpan.FromMinutes(30);

            for (var time = start; time < end; time = time.Add(interval))
            {
                bool occupied = appointments.Any(a => a.StartTime == TimeOnly.FromTimeSpan(time - start + TimeSpan.FromHours(start.Hour) + TimeSpan.FromMinutes(start.Minute)));
                _slots.Add(new AppointmentSlot
                {
                    StartTime = TimeOnly.FromTimeSpan(time - start + TimeSpan.FromHours(start.Hour) + TimeSpan.FromMinutes(start.Minute)),
                    IsOccupied = occupied
                });
            }

            SlotsListBox.ItemsSource = _slots;
        }
        private void AppointmentDatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not DatePicker datePicker)
                return;

            if (datePicker.Template.FindName("PART_TextBox", datePicker) is DatePickerTextBox textBox)
            {
                textBox.IsReadOnly = true;
                textBox.Cursor = Cursors.Hand;
                textBox.PreviewMouseLeftButtonDown -= DatePickerTextBox_PreviewMouseLeftButtonDown;
                textBox.PreviewMouseLeftButtonDown += DatePickerTextBox_PreviewMouseLeftButtonDown;
            }
        }

        private void DatePickerTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AppointmentDatePicker.IsDropDownOpen = true;
            e.Handled = true;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPatient == null)
            {
                MessageBox.Show("Выберите пациента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DoctorComboBox.SelectedItem is not Doctor doctor ||
                AppointmentDatePicker.SelectedDate is not DateTime selectedDate ||
                SlotsListBox.SelectedItem is not AppointmentSlot selectedSlot ||
                selectedSlot.IsOccupied)
            {
                MessageBox.Show("Выберите свободное время.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();

            var appointment = new Appointment
            {
                DoctorId = doctor.DoctorId,
                PatientId = CurrentPatient.PatientId,
                AppointmentDate = DateOnly.FromDateTime(selectedDate),
                StartTime = selectedSlot.StartTime
            };

            await appointmentRepository.AddAsync(appointment);

            MessageBox.Show("Запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadSlotsAsync();
        }
    }

    public class AppointmentSlot
    {
        public TimeOnly StartTime { get; set; }
        public bool IsOccupied { get; set; }

        public override string ToString()
        {
            return $"{StartTime:hh\\:mm} - {(IsOccupied ? "Занято" : "Свободно")}";
        }
    }
}
