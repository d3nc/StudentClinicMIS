using StudentClinicMIS.Data;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Data.Repositories;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StudentClinicMIS.Views
{
    public partial class AdminMainWindow : Window
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly PolyclinicContext _context = new PolyclinicContext();

        public AdminMainWindow()
        {
            InitializeComponent();

            _patientRepository = App.AppHost.Services.GetService(typeof(IPatientRepository)) as IPatientRepository;
            _appointmentRepository = App.AppHost.Services.GetService(typeof(IAppointmentRepository)) as IAppointmentRepository;

            _ = LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            PatientsDataGrid.ItemsSource = patients;
        }

        private void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddPatientWindow(_patientRepository);
            if (addWindow.ShowDialog() == true)
            {
                _ = LoadPatientsAsync();
            }
        }

        private void EditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var editWindow = new EditPatientWindow(selectedPatient, _patientRepository);
                if (editWindow.ShowDialog() == true)
                {
                    _ = LoadPatientsAsync();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пациента для редактирования.");
            }
        }

        private async void DeletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого пациента?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await _patientRepository.DeleteAsync(selectedPatient.PatientId);
                    await LoadPatientsAsync();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пациента для удаления.");
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchTextBox?.Text?.Trim()?.ToLower() ?? "";
            var allPatients = await _patientRepository.GetAllAsync();

            var filtered = allPatients.Where(p =>
                p.FirstName.ToLower().Contains(searchText) ||
                p.LastName.ToLower().Contains(searchText) ||
                p.MiddleName.ToLower().Contains(searchText) ||
                p.Phone.ToLower().Contains(searchText)
            ).ToList();

            PatientsDataGrid.ItemsSource = filtered;
        }

        private async void ViewAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var appointments = await _appointmentRepository.GetByPatientIdAsync(selectedPatient.PatientId);
                var appointmentsWindow = new PatientAppointmentsWindow(appointments);
                appointmentsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите пациента для просмотра истории посещений.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
