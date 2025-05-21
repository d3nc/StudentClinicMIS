using CommunityToolkit.Mvvm.Input;
using StudentClinicMIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace StudentClinicMIS.ViewModels.Doctor
{
    public class DoctorMainViewModel : BaseViewModel
    {
        private readonly PolyclinicContext _context;
        private Appointment _selectedAppointment;

        public Models.Doctor CurrentDoctor { get; }
        public List<Appointment> TodayAppointments { get; private set; }
        public List<DoctorScheduleEntity> DoctorSchedules { get; private set; }

        public DateTime NewRecordDate { get; set; } = DateTime.Today;
        public string NewDiagnosis { get; set; }
        public string NewRecommendations { get; set; }

        public Appointment SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                _selectedAppointment = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPatientSelected));
            }
        }

        public bool IsPatientSelected => SelectedAppointment != null;

        public RelayCommand SaveRecordCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public DoctorMainViewModel(Models.Doctor doctor)
        {
            _context = new PolyclinicContext();
            CurrentDoctor = doctor;

            LoadData();

            SaveRecordCommand = new RelayCommand(() => SaveMedicalRecord());
            RefreshCommand = new RelayCommand(() => LoadData());
        }

        private void LoadData()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            TodayAppointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == CurrentDoctor.DoctorId &&
                           a.AppointmentDate == today)
                .OrderBy(a => a.StartTime)
                .ToList();

            DoctorSchedules = _context.DoctorSchedules1
                .Include(ds => ds.Room)
                .Where(ds => ds.DoctorId == CurrentDoctor.DoctorId)
                .OrderBy(ds => ds.DayOfWeek)
                .ThenBy(ds => ds.StartTime)
                .ToList();

            OnPropertyChanged(nameof(TodayAppointments));
            OnPropertyChanged(nameof(DoctorSchedules));
        }

        private void SaveMedicalRecord()
        {
            if (SelectedAppointment == null) return;

            try
            {
                var record = new MedicalRecord
                {
                    PatientId = SelectedAppointment.PatientId,
                    DoctorId = CurrentDoctor.DoctorId,
                    RecordDate = NewRecordDate,
                    Diagnosis = NewDiagnosis,
                    Treatment = NewRecommendations,
                    Symptoms = SelectedAppointment.Purpose,
                    Notes = $"Запись создана {DateTime.Now:dd.MM.yyyy HH:mm}"
                };

                _context.MedicalRecords.Add(record);
                _context.SaveChanges();

                NewDiagnosis = string.Empty;
                NewRecommendations = string.Empty;
                OnPropertyChanged(nameof(NewDiagnosis));
                OnPropertyChanged(nameof(NewRecommendations));

                MessageBox.Show("Запись успешно сохранена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}