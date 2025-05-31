using CommunityToolkit.Mvvm.Input;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DoctorEntity = StudentClinicMIS.Models.Doctor;

namespace StudentClinicMIS.ViewModels.Admin
{
    public class AddEditDoctorViewModel : INotifyPropertyChanged
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISpecializationRepository _specializationRepository;

        public AddEditDoctorViewModel(
            IDoctorRepository doctorRepository,
            IEmployeeRepository employeeRepository,
            ISpecializationRepository specializationRepository)
        {
            _doctorRepository = doctorRepository;
            _employeeRepository = employeeRepository;
            _specializationRepository = specializationRepository;

            LoadData();

            SaveCommand = new RelayCommand(SaveDoctor, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void LoadData()
        {
            Employees = new ObservableCollection<Employee>(await _employeeRepository.GetAllAsync());
            Specializations = new ObservableCollection<Specialization>(await _specializationRepository.GetAllAsync());
        }

        private DoctorEntity? _doctor;
        public DoctorEntity? Doctor
        {
            get => _doctor;
            set
            {
                _doctor = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Employee> _employees = new();
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Specialization> _specializations = new();
        public ObservableCollection<Specialization> Specializations
        {
            get => _specializations;
            set { _specializations = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? RequestClose;

        private async void SaveDoctor()
        {
            try
            {
                if (Doctor == null) return;

                if (Doctor.DoctorId == 0)
                    await _doctorRepository.AddAsync(Doctor);
                else
                    await _doctorRepository.UpdateAsync(Doctor);

                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private bool CanSave()
        {
            return Doctor != null &&
                   Doctor.EmployeeId > 0 &&
                   !string.IsNullOrWhiteSpace(Doctor.Qualification);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}