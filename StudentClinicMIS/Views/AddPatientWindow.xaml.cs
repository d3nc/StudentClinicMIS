using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using System.Windows;

namespace StudentClinicMIS.Views
{
    public partial class AddPatientWindow : Window
    {
        private readonly IPatientRepository _patientRepository;
        private readonly Patient _patient;

        public AddPatientWindow(IPatientRepository patientRepository, Patient? patient = null)
        {
            InitializeComponent();
            _patientRepository = patientRepository;
            _patient = patient ?? new Patient();

            // Предзаполнить поля, если редактируем
            FirstNameBox.Text = _patient.FirstName;
            LastNameBox.Text = _patient.LastName;
            MiddleNameBox.Text = _patient.MiddleName;
            PhoneBox.Text = _patient.Phone;
            if (_patient.BirthDate.HasValue)
                BirthDatePicker.SelectedDate = _patient.BirthDate.Value.ToDateTime(new System.TimeOnly(0));
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _patient.FirstName = FirstNameBox.Text;
            _patient.LastName = LastNameBox.Text;
            _patient.MiddleName = MiddleNameBox.Text;
            _patient.Phone = PhoneBox.Text;
            _patient.BirthDate = BirthDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value)
                : null;

            if (_patient.PatientId == 0)
                await _patientRepository.AddAsync(_patient);
            else
                await _patientRepository.UpdateAsync(_patient);

            DialogResult = true;
            Close();
        }
    }
}
