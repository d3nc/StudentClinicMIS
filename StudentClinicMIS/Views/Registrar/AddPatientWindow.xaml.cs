using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Views
{
    public partial class AddPatientWindow : Window
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEnumerable<Gender> _genders;
        private readonly Patient _patient;
        private bool _isCancelling = false;

        public AddPatientWindow(
            IPatientRepository patientRepository,
            IFacultyRepository facultyRepository,
            IGroupRepository groupRepository,
            IEnumerable<Gender> genders,
            Patient patient = null)
        {
            InitializeComponent();
            _patientRepository = patientRepository;
            _facultyRepository = facultyRepository;
            _groupRepository = groupRepository;
            _genders = genders ?? throw new ArgumentNullException(nameof(genders));
            _patient = patient ?? new Patient();

            InitializeFields();
            LoadGenders();
            LoadFaculties();
        }

        private void LoadGenders()
        {
            try
            {
                GenderComboBox.ItemsSource = _genders;
                GenderComboBox.DisplayMemberPath = "Name";
                GenderComboBox.SelectedValuePath = "GenderId";

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
                FacultyComboBox.DisplayMemberPath = "Name";
                FacultyComboBox.SelectedValuePath = "FacultyId";

                if (_patient.StudentCard?.Group?.FacultyId.HasValue == true)
                {
                    FacultyComboBox.SelectedValue = _patient.StudentCard.Group.FacultyId.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки факультетов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void FacultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FacultyComboBox.SelectedValue is int facultyId)
            {
                await LoadGroupsByFaculty(facultyId);
                GroupComboBox.IsEnabled = true;
            }
            else
            {
                GroupComboBox.ItemsSource = null;
                GroupComboBox.IsEnabled = false;
            }
        }

        private async Task LoadGroupsByFaculty(int facultyId)
        {
            try
            {
                var groups = await _groupRepository.GetByFacultyIdAsync(facultyId);
                GroupComboBox.ItemsSource = groups;

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

        private void InitializeFields()
        {
            FirstNameBox.Text = _patient.FirstName;
            LastNameBox.Text = _patient.LastName;
            MiddleNameBox.Text = _patient.MiddleName;
            PhoneBox.Text = string.IsNullOrEmpty(_patient.Phone) ? "+7 (" : _patient.Phone;

            if (_patient.BirthDate.HasValue)
                BirthDateTextBox.Text = _patient.BirthDate.Value.ToString("dd.MM.yyyy");
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

        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void BirthDateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (_isCancelling) return;
            if (!char.IsDigit(e.Text[0]) && e.Text != ".") e.Handled = true;

            if (BirthDateTextBox.Text.Length == 2 || BirthDateTextBox.Text.Length == 5)
            {
                BirthDateTextBox.Text += ".";
                BirthDateTextBox.CaretIndex = BirthDateTextBox.Text.Length;
            }

            if (BirthDateTextBox.Text.Length >= 10) e.Handled = true;
        }

        private void BirthDateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isCancelling) return;

            if (BirthDateTextBox.Text.Length == 2 && !BirthDateTextBox.Text.Contains("."))
            {
                BirthDateTextBox.Text += ".";
                BirthDateTextBox.CaretIndex = BirthDateTextBox.Text.Length;
            }
            else if (BirthDateTextBox.Text.Length == 5 && BirthDateTextBox.Text.Count(c => c == '.') < 2)
            {
                BirthDateTextBox.Text += ".";
                BirthDateTextBox.CaretIndex = BirthDateTextBox.Text.Length;
            }
        }

        private void BirthDateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isCancelling) return;

            if (!DateTime.TryParse(BirthDateTextBox.Text, out _))
            {
                BirthDateTextBox.Text = "";
                MessageBox.Show("Введите дату в формате ДД.ММ.ГГГГ",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isCancelling = true;
            DialogResult = false;
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isCancelling || !ValidateInputs()) return;

            _patient.FirstName = FirstNameBox.Text.Trim();
            _patient.LastName = LastNameBox.Text.Trim();
            _patient.MiddleName = MiddleNameBox.Text.Trim();
            _patient.Phone = PhoneBox.Text.Trim();
            _patient.GenderId = (int?)GenderComboBox.SelectedValue;

            if (DateTime.TryParse(BirthDateTextBox.Text, out var birthDate))
                _patient.BirthDate = DateOnly.FromDateTime(birthDate);

            if (GroupComboBox.SelectedValue is int groupId)
            {
                _patient.StudentCard ??= new StudentCard();
                _patient.StudentCard.GroupId = groupId;
            }

            try
            {
                if (_patient.PatientId == 0)
                    await _patientRepository.AddAsync(_patient);
                else
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

            if (!DateTime.TryParse(BirthDateTextBox.Text, out var date) || date > DateTime.Today)
                return ShowValidation("Введите корректную дату рождения в формате ДД.ММ.ГГГГ");

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
    }
}