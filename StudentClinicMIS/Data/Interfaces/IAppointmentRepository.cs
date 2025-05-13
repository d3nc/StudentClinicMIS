﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Data.Interfaces
{
    public interface IAppointmentRepository
    {   
        Task<List<Appointment>> GetByPatientIdAsync(int patientId);
        Task AddAsync(Appointment appointment);
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateOnly date);

    }
}
