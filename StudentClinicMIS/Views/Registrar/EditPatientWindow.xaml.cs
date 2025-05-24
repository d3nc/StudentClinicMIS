using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Views
{
    public partial class EditPatientWindow : Window
    {
        private readonly Patient _patient;
        private readonly IPatientRepository _patientRepository;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEnumerable<Gender> _genders;
        private bool _isCancelling = false;

        public EditPatientWindow(
            Patient patient,
            IPatientRepository patientRepository,
            IFacultyRepository facultyRepository,
            IGroupRepository groupRepository,
            IEnumerable<Gender> genders)
        {
            InitializeComponent();
            _patient = patient;
            _patientRepository = patientRepository;
            _facultyRepository = facultyRepository;
            _groupRepository = groupRepository;
            _genders = genders;

            InitializeFields();
            LoadGenders();
            LoadFaculties();
        }

        private void InitializeFields()
        {
            FirstNameBox.Text = _patient.FirstName;
            LastNameBox.Text = _patient.LastName;
            MiddleNameBox.Text = _patient.MiddleName;
            PhoneBox.Text = string.IsNullOrEmpty(_patient.Phone) ? "+7 (" : _patient.Phone;

            if (_patient.BirthDate.HasValue)
                BirthDatePicker.SelectedDate = _patient.BirthDate.Value.ToDateTime(TimeOnly.MinValue);

            if (_patient.GenderId.HasValue)
                GenderComboBox.SelectedValue = _patient.GenderId.Value;
        }

        private void LoadGenders()
        {
            try
            {
                GenderComboBox.ItemsSource = _genders;
                if (_patient.GenderId.HasValue)
                    GenderComboBox.SelectedValue = _patient.GenderId.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка полов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadFaculties()
        {
            try
            {
                var faculties = await _facultyRepository.GetAllAsync();
                FacultyComboBox.ItemsSource = faculties;

                if (_patient.StudentCard?.Group?.FacultyId.HasValue == true)
                {
                    FacultyComboBox.SelectedValue = _patient.StudentCard.Group.FacultyId.Value;
                    await LoadGroupsByFaculty(_patient.StudentCard.Group.FacultyId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки факультетов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadGroupsByFaculty(int facultyId)
        {
            try
            {
                var groups = await _groupRepository.GetByFacultyIdAsync(facultyId);
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.IsEnabled = true;

                if (_patient.StudentCard?.GroupId.HasValue == true)
                {
                    GroupComboBox.SelectedValue = _patient.StudentCard.GroupId.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void FacultyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FacultyComboBox.SelectedValue is int facultyId)
            {
                await LoadGroupsByFaculty(facultyId);
            }
            else
            {
                GroupComboBox.ItemsSource = null;
                GroupComboBox.IsEnabled = false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void TextInput_OnlyLetters(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[а-яА-ЯёЁa-zA-Z- ]+$");
        }

        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (_isCancelling) return;
            if (!char.IsDigit(e.Text[0])) { e.Handled = true; return; }

            string currentText = new string(PhoneBox.Text.Where(char.IsDigit).ToArray());
            if (currentText.Length >= 11) e.Handled = true;
        }

        private void PhoneBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_isCancelling) return;

            string digits = new string(PhoneBox.Text.Where(char.IsDigit).ToArray());
            if (digits.Length < 1 || !digits.StartsWith("7")) return;

            string formatted = "+7 (";
            if (digits.Length > 1) formatted += digits.Substring(1, Math.Min(3, digits.Length - 1));
            if (digits.Length > 4) formatted += ") " + digits.Substring(4, Math.Min(3, digits.Length - 4));
            if (digits.Length > 7) formatted += "-" + digits.Substring(7, Math.Min(2, digits.Length - 7));
            if (digits.Length > 9) formatted += "-" + digits.Substring(9, Math.Min(2, digits.Length - 9));

            PhoneBox.Text = formatted;
            PhoneBox.CaretIndex = PhoneBox.Text.Length;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isCancelling || !ValidateInputs()) return;

            _patient.FirstName = FirstNameBox.Text.Trim();
            _patient.LastName = LastNameBox.Text.Trim();
            _patient.MiddleName = MiddleNameBox.Text.Trim();
            _patient.Phone = PhoneBox.Text.Trim();
            _patient.GenderId = (int?)GenderComboBox.SelectedValue;

            if (BirthDatePicker.SelectedDate.HasValue)
                _patient.BirthDate = DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value);

            // Обновляем группу
            if (GroupComboBox.SelectedValue is int groupId)
            {
                _patient.StudentCard ??= new StudentCard();
                _patient.StudentCard.GroupId = groupId;
            }

            try
            {
                await _patientRepository.UpdateAsync(_patient);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
                return ShowValidation("Поле 'Фамилия' обязательно для заполнения");

            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
                return ShowValidation("Поле 'Имя' обязательно для заполнения");

            if (GenderComboBox.SelectedValue == null)
                return ShowValidation("Выберите пол");

            string phoneDigits = new string(PhoneBox.Text.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length != 11 || !phoneDigits.StartsWith("7"))
                return ShowValidation("Телефон должен быть в формате +7 XXX XXX-XX-XX");

            if (!BirthDatePicker.SelectedDate.HasValue || BirthDatePicker.SelectedDate > DateTime.Today)
                return ShowValidation("Введите корректную дату рождения");

            if (FacultyComboBox.SelectedValue == null)
                return ShowValidation("Выберите факультет");

            if (GroupComboBox.SelectedValue == null)
                return ShowValidation("Выберите группу");

            return true;
        }

        private bool ShowValidation(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isCancelling = true;
            DialogResult = false;
            Close();
        }
    }
}