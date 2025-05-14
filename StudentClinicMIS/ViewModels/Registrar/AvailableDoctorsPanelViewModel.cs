using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentClinicMIS.ViewModels.Registrar
{
    public class AvailableDoctorsPanelViewModel : INotifyPropertyChanged
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;

        public ObservableCollection<Department> Departments { get; } = new();
        public ObservableCollection<Doctor> Doctors { get; } = new();
        public ObservableCollection<DoctorSlotViewModel> DoctorSlots { get; } = new();

        private Department? _selectedDepartment;
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (SetField(ref _selectedDepartment, value))
                {
                    LoadDoctorsAsync();
                }
            }
        }

        private Doctor? _selectedDoctor;
        public Doctor? SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                if (SetField(ref _selectedDoctor, value))
                {
                    LoadDoctorSlotsAsync();
                }
            }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetField(ref _selectedDate, value))
                {
                    LoadDoctorSlotsAsync();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AvailableDoctorsPanelViewModel(IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository)
        {
            _departmentRepository = departmentRepository;
            _doctorRepository = doctorRepository;

            _ = LoadDepartmentsAsync();
        }

        private async Task LoadDepartmentsAsync()
        {
            Departments.Clear();
            var list = await _departmentRepository.GetAllAsync();
            foreach (var dept in list)
                Departments.Add(dept);
        }

        private async Task LoadDoctorsAsync()
        {
            Doctors.Clear();
            if (SelectedDepartment == null)
                return;

            var list = await _doctorRepository.GetByDepartmentIdAsync(SelectedDepartment.DepartmentId);
            foreach (var doctor in list)
                Doctors.Add(doctor);
        }

        private async Task LoadDoctorSlotsAsync()
        {
            DoctorSlots.Clear();
            if (SelectedDoctor == null || SelectedDate == null)
                return;

            var dayOfWeek = SelectedDate.Value.DayOfWeek;
            var schedule = await _doctorRepository.GetScheduleForDoctorAsync(SelectedDoctor.DoctorId, dayOfWeek);
            if (schedule != null)
            {
                var slotVm = new DoctorSlotViewModel(SelectedDoctor, schedule, DateOnly.FromDateTime(SelectedDate.Value)); // ✅ DateOnly

                DoctorSlots.Add(slotVm);
            }
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
