using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views.Registrar;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentClinicMIS.Views.Registrar
{
    public partial class RegistrarMainWindow : Window
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private ObservableCollection<Patient> _patients;

        public Patient? SelectedPatient { get; private set; }

        public RegistrarMainWindow(IPatientRepository patientRepository, IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _patients = new ObservableCollection<Patient>();

            PatientsDataGrid.ItemsSource = _patients;
            Loaded += async (s, e) => await LoadPatientsAsync();
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
            PatientsDataGrid.SelectionChanged += PatientsDataGrid_SelectionChanged;
            PatientsDataGrid.MouseDoubleClick += PatientsDataGrid_MouseDoubleClick;
        }

        private async Task LoadPatientsAsync()
        {
            try
            {
                StatusText.Text = "Загрузка пациентов...";
                var patients = await _patientRepository.GetAllAsync();
                _patients.Clear();
                foreach (var patient in patients.OrderBy(p => p.LastName))
                {
                    _patients.Add(patient);
                }
                StatusText.Text = $"Загружено {_patients.Count} пациентов";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка загрузки пациентов";
                ShowErrorMessage($"Ошибка загрузки пациентов: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                PatientsDataGrid.ItemsSource = _patients;
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                await LoadPatientsAsync();
                return;
            }

            try
            {
                var filtered = _patients.Where(p =>
                    (p.LastName?.ToLower().Contains(query) == true ||
                     p.FirstName?.ToLower().Contains(query) == true ||
                     p.MiddleName?.ToLower().Contains(query) == true ||
                     p.Phone?.Contains(query) == true ||
                     p.InsuranceNumber?.Contains(query) == true)
                ).ToList();

                PatientsDataGrid.ItemsSource = filtered;
                StatusText.Text = $"Найдено {filtered.Count} пациентов";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка поиска";
                ShowErrorMessage($"Ошибка поиска: {ex.Message}");
            }
        }

        private async void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWindow = new AddPatientWindow(_patientRepository);
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

        private void PatientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPatient = PatientsDataGrid.SelectedItem as Patient;
        }

        private void PatientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient patient)
            {
                SelectedPatient = patient;
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

        private async void EditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
                {
                    var editWindow = new EditPatientWindow(selectedPatient, _patientRepository);
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
