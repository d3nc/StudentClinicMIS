using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudentClinicMIS
{
    public partial class MainWindow : Window
    {
        private readonly IPatientRepository _patientRepository;
        private List<Patient> _allPatients = new();

        public MainWindow(IPatientRepository patientRepository)
        {
            InitializeComponent();
            _patientRepository = patientRepository;
            Loaded += async (_, __) => await LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            _allPatients = await _patientRepository.GetAllAsync();
            PatientsDataGrid.ItemsSource = _allPatients;
        }

        private async void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddPatientWindow(_patientRepository);
            if (addWindow.ShowDialog() == true)
            {
                await LoadPatientsAsync();
            }
        }

        private async void EditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var editWindow = new EditPatientWindow(selectedPatient, _patientRepository);
                if (editWindow.ShowDialog() == true)
                {
                    await LoadPatientsAsync();
                }
            }
            else
            {
                MessageBox.Show("Выберите пациента для редактирования.");
            }
        }

        private async void DeletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить пациента?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await _patientRepository.DeleteAsync(selectedPatient.PatientId);
                    await LoadPatientsAsync();
                }
            }
            else
            {
                MessageBox.Show("Выберите пациента для удаления.");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            var filtered = _allPatients.Where(p =>
                (p.FirstName?.ToLower().Contains(query) ?? false) ||
                (p.LastName?.ToLower().Contains(query) ?? false) ||
                (p.MiddleName?.ToLower().Contains(query) ?? false) ||
                (p.Phone?.ToLower().Contains(query) ?? false)
            ).ToList();

            PatientsDataGrid.ItemsSource = filtered;
        }
    }
}
