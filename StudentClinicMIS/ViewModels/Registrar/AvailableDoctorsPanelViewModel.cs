using StudentClinicMIS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using StudentClinicMIS.Data.Interfaces;
using System.ComponentModel;
using StudentClinicMIS.ViewModels.Doctor;

namespace StudentClinicMIS.ViewModels.Registrar
{
    public class AvailableDoctorsPanelViewModel : BaseViewModel
    {
        private readonly IDoctorRepository _doctorRepository;
        private Models.Doctor _selectedDoctor;

        public ObservableCollection<Models.Doctor> AvailableDoctors { get; } = new();
        public Models.Doctor SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                _selectedDoctor = value;
                OnPropertyChanged();
                LoadDoctorSchedule();
            }
        }

        public ObservableCollection<DoctorSlotViewModel> DoctorSlots { get; } = new();

        public AvailableDoctorsPanelViewModel(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
            LoadDoctors();
        }

        private async void LoadDoctors()
        {
            var doctors = await _doctorRepository.GetByDepartmentIdAsync(1); // Пример ID отделения
            AvailableDoctors.Clear();
            foreach (var doctor in doctors)
            {
                AvailableDoctors.Add(doctor);
            }
        }

        private async void LoadDoctorSchedule()
        {
            if (SelectedDoctor == null) return;

            DoctorSlots.Clear();
            var schedule = await _doctorRepository.GetScheduleForDoctorAsync(
                SelectedDoctor.DoctorId,
                DateTime.Today.DayOfWeek);

            if (schedule != null)
            {
                var slotViewModel = new DoctorSlotViewModel(
                    SelectedDoctor,
                    schedule,
                    DateOnly.FromDateTime(DateTime.Today));

                DoctorSlots.Add(slotViewModel);
            }
        }
    }
}