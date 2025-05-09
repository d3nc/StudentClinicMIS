using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using System;
using System.Windows;

namespace StudentClinicMIS.Views
{
    public partial class EditPatientWindow : Window
    {
        private readonly Patient _patient;
        private readonly IPatientRepository _patientRepository;

        public EditPatientWindow(Patient patient, IPatientRepository patientRepository)
        {
            InitializeComponent();
            _patient = patient;
            _patientRepository = patientRepository;

            FirstNameBox.Text = patient.FirstName;
            LastNameBox.Text = patient.LastName;
            MiddleNameBox.Text = patient.MiddleName;
            PhoneBox.Text = patient.Phone;
            if (patient.BirthDate.HasValue)
                BirthDatePicker.SelectedDate = patient.BirthDate.Value.ToDateTime(TimeOnly.MinValue);
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

            await _patientRepository.UpdateAsync(_patient);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}