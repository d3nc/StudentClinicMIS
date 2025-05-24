using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views.Admin;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using System.Threading.Tasks;
using System;

namespace StudentClinicMIS.ViewModels.Admin
{
    public class DoctorsPageViewModel : INotifyPropertyChanged
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IDepartmentRepository _departmentRepository;

        private string _searchText;
        private Specialization _selectedSpecialization;
        private Department _selectedDepartment;
        private Models.Doctor _selectedDoctor; // Явное указание пространства имен

        public ObservableCollection<Models.Doctor> AllDoctors { get; private set; }
        public ObservableCollection<Models.Doctor> FilteredDoctors { get; private set; }
        public ObservableCollection<Specialization> Specializations { get; private set; }
        public ObservableCollection<Department> Departments { get; private set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public Specialization SelectedSpecialization
        {
            get => _selectedSpecialization;
            set
            {
                _selectedSpecialization = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public Models.Doctor SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                _selectedDoctor = value;
                OnPropertyChanged();
                ((RelayCommand)EditDoctorCommand).NotifyCanExecuteChanged();
                ((RelayCommand)DeleteDoctorCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand ClearFiltersCommand { get; }
        public ICommand AddDoctorCommand { get; }
        public ICommand EditDoctorCommand { get; }
        public ICommand DeleteDoctorCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DoctorsPageViewModel(
            IDoctorRepository doctorRepository,
            ISpecializationRepository specializationRepository,
            IDepartmentRepository departmentRepository)
        {
            _doctorRepository = doctorRepository;
            _specializationRepository = specializationRepository;
            _departmentRepository = departmentRepository;

            AllDoctors = new ObservableCollection<Models.Doctor>();
            FilteredDoctors = new ObservableCollection<Models.Doctor>();
            Specializations = new ObservableCollection<Specialization>();
            Departments = new ObservableCollection<Department>();

            LoadDataAsync();

            ClearFiltersCommand = new RelayCommand(ClearFilters);
            AddDoctorCommand = new RelayCommand(AddDoctor);
            EditDoctorCommand = new RelayCommand(EditDoctor, () => SelectedDoctor != null);
            DeleteDoctorCommand = new RelayCommand(DeleteDoctor, () => SelectedDoctor != null);
        }

        private async void LoadDataAsync()
        {
            try
            {
                var doctors = await _doctorRepository.GetAllAsync();
                var specializations = await _specializationRepository.GetAllAsync();
                var departments = await _departmentRepository.GetAllAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AllDoctors.Clear();
                    foreach (var doctor in doctors)
                    {
                        AllDoctors.Add(doctor);
                    }

                    Specializations.Clear();
                    foreach (var spec in specializations)
                    {
                        Specializations.Add(spec);
                    }

                    Departments.Clear();
                    foreach (var dept in departments)
                    {
                        Departments.Add(dept);
                    }

                    ApplyFilters();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            FilteredDoctors.Clear();

            var query = AllDoctors.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(d =>
                    d.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedSpecialization != null)
            {
                query = query.Where(d =>
                    d.Specialization?.SpecializationId == SelectedSpecialization.SpecializationId);
            }

            if (SelectedDepartment != null)
            {
                query = query.Where(d =>
                    d.Employee?.DepartmentId == SelectedDepartment.DepartmentId);
            }

            foreach (var doctor in query)
            {
                FilteredDoctors.Add(doctor);
            }
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedSpecialization = null;
            SelectedDepartment = null;
        }

        private void AddDoctor()
        {
            var newDoctor = new Models.Doctor
            {
                Employee = new Employee(),
                Specialization = Specializations.FirstOrDefault()
            };

            var window = new AddEditDoctorWindow(newDoctor, Specializations, Departments);
            if (window.ShowDialog() == true)
            {
                AllDoctors.Add(newDoctor);
                ApplyFilters();
            }
        }

        private void EditDoctor()
        {
            if (SelectedDoctor == null) return;

            var window = new AddEditDoctorWindow(SelectedDoctor, Specializations, Departments);
            if (window.ShowDialog() == true)
            {
                ApplyFilters();
            }
        }

        private async void DeleteDoctor()
        {
            if (SelectedDoctor == null) return;

            var result = MessageBox.Show(
                $"Удалить врача {SelectedDoctor.FullName}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _doctorRepository.DeleteAsync(SelectedDoctor.DoctorId);
                    AllDoctors.Remove(SelectedDoctor);
                    FilteredDoctors.Remove(SelectedDoctor);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}