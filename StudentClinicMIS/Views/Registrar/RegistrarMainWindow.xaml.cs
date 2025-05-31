using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Data.Repositories;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Views.Registrar
{
    public partial class RegistrarMainWindow : Window
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ObservableCollection<Patient> _patients = new();
        private List<Patient> _allPatients = new();

        public Patient? SelectedPatient => PatientsDataGrid.SelectedItem as Patient;

        public RegistrarMainWindow(
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IFacultyRepository facultyRepository,
            IGroupRepository groupRepository)
        {
            InitializeComponent();
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _facultyRepository = facultyRepository;
            _groupRepository = groupRepository;
            PatientsDataGrid.ItemsSource = _patients;
            Loaded += async (_, _) => await LoadPatientsAsync();
            PatientsDataGrid.MouseDoubleClick += PatientsDataGrid_MouseDoubleClick;
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
        }

        private async void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWindow = new AddPatientWindow(
                    _patientRepository,
                    _facultyRepository,
                    _groupRepository,
                    GenderComboBox.ItemsSource as IEnumerable<Gender>,
                    null);

                if (addWindow.ShowDialog() == true)
                {
                    await LoadPatientsAsync();
                    ShowInfoMessage("Пациент успешно добавлен.");
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка добавления пациента";
                ShowErrorMessage($"Ошибка добавления пациента: {ex.Message}");
            }
        }
        private async Task LoadPatientsAsync()
        {
            try
            {
                StatusText.Text = "Загрузка пациентов...";
                var patients = await _patientRepository.GetAllAsync();

                _allPatients = patients.ToList();

                var genders = _allPatients
                    .Where(p => p.Gender != null)
                    .Select(p => p.Gender)
                    .Distinct()
                    .OrderBy(g => g.Name)
                    .ToList();
                GenderComboBox.ItemsSource = genders;

                var faculties = _allPatients
                    .Where(p => p.StudentCard?.Group?.Faculty != null)
                    .Select(p => p.StudentCard.Group.Faculty)
                    .Distinct()
                    .OrderBy(f => f.Name)
                    .ToList();
                FacultyComboBox.ItemsSource = faculties;

                ApplyFilters();
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка загрузки пациентов";
                ShowErrorMessage($"Ошибка загрузки пациентов: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            string query = SearchTextBox.Text.Trim().ToLower();
            string? selectedGender = GenderComboBox.SelectedValue as string;
            string? selectedFaculty = FacultyComboBox.SelectedValue as string;

            var filtered = _allPatients.Where(p =>
                (string.IsNullOrWhiteSpace(query) ||
                    (p.LastName?.ToLower().Contains(query) == true ||
                     p.FirstName?.ToLower().Contains(query) == true ||
                     p.MiddleName?.ToLower().Contains(query) == true ||
                     p.Phone?.Contains(query) == true ||
                     p.InsuranceNumber?.Contains(query) == true)) &&
                (string.IsNullOrWhiteSpace(selectedGender) || p.Gender?.Name == selectedGender) &&
                (string.IsNullOrWhiteSpace(selectedFaculty) || p.StudentCard?.Group?.Faculty?.ShortName == selectedFaculty)
            ).OrderBy(p => p.LastName).ToList();

            _patients.Clear();
            foreach (var patient in filtered)
            {
                _patients.Add(patient);
            }

            StatusText.Text = $"Найдено {_patients.Count} пациентов";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private async void EditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
                {
                    var editWindow = new EditPatientWindow(
                        selectedPatient,
                        _patientRepository,
                        _facultyRepository,
                        _groupRepository,
                        GenderComboBox.ItemsSource as IEnumerable<Gender>
                    );

                    if (editWindow.ShowDialog() == true)
                    {
                        await LoadPatientsAsync();
                        ShowInfoMessage("Данные пациента успешно обновлены.");
                    }
                }
                else
                {
                    ShowInfoMessage("Выберите пациента для редактирования.");
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка редактирования";
                ShowErrorMessage($"Ошибка редактирования: {ex.Message}");
            }
        }

        private async void DeletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
                {
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить пациента {selectedPatient.LastName} {selectedPatient.FirstName}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        await _patientRepository.DeleteAsync(selectedPatient.PatientId);
                        await LoadPatientsAsync();
                        ShowInfoMessage("Пациент успешно удален.");
                    }
                }
                else
                {
                    ShowInfoMessage("Выберите пациента для удаления.");
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка удаления";
                ShowErrorMessage($"Ошибка удаления: {ex.Message}");
            }
        }

        private async void ViewAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
                {
                    var appointments = await _appointmentRepository.GetByPatientIdAsync(selectedPatient.PatientId);
                    var historyWindow = new PatientAppointmentsWindow(appointments);
                    historyWindow.ShowDialog();
                }
                else
                {
                    ShowInfoMessage("Выберите пациента для просмотра истории приёмов.");
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка загрузки приёмов";
                ShowErrorMessage($"Ошибка загрузки приёмов: {ex.Message}");
            }
        }

        private void PatientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient patient)
            {
                e.Handled = true;
                foreach (TabItem tab in MainTabControl.Items)
                {
                    if (tab.Header?.ToString() == "Запись на приём")
                    {
                        MainTabControl.SelectedItem = tab;
                        if (tab.Content is AvailableDoctorsPanel panel)
                        {
                            panel.CurrentPatient = SelectedPatient;
                            panel.UpdatePatientInfo();
                        }
                        break;
                    }
                }
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl && tabControl.SelectedItem is TabItem tabItem)
            {
                if (tabItem.Header?.ToString() == "Запись на приём")
                {
                    if (tabItem.Content is AvailableDoctorsPanel panel)
                    {
                        panel.CurrentPatient = SelectedPatient;
                        panel.UpdatePatientInfo();
                    }
                }
            }
        }

        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}