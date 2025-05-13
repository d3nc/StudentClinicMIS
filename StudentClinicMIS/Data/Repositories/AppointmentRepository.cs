using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AppointmentRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(int patientId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            return await context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Employee)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            return await context.Doctors
                .Include(d => d.Employee)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateOnly date)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            return await context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .ToListAsync();
        }

        public async Task<List<DoctorWithSlotsViewModel>> GetAvailableDoctorsAsync(int departmentId, DateOnly date)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            var weekday = date.DayOfWeek.ToString();
            var doctors = await context.Doctors
                .Include(d => d.Employee)
                .Include(d => d.Specialization)
                .Include(d => d.DoctorSchedule1s.Where(s => s.DayOfWeek == weekday))
                .Where(d => d.Departments.Any(dep => dep.DepartmentId == departmentId))
                .ToListAsync();

            var appointments = await context.Appointments
                .Where(a => a.AppointmentDate == date)
                .ToListAsync();

            var results = new List<DoctorWithSlotsViewModel>();

            foreach (var doctor in doctors)
            {
                var fullName = $"{doctor.Employee.LastName} {doctor.Employee.FirstName} {doctor.Employee.MiddleName}";
                var specialization = doctor.Specialization?.Name ?? "";

                var schedule = doctor.DoctorSchedule1s.FirstOrDefault();
                if (schedule == null) continue;

                var freeSlots = GenerateTimeSlots(schedule.StartTime, schedule.EndTime, TimeSpan.FromMinutes(30));

                var busyTimes = appointments
                    .Where(a => a.DoctorId == doctor.DoctorId)
                    .Select(a => a.StartTime)
                    .ToHashSet();

                var available = freeSlots.Where(t => !busyTimes.Contains(t)).ToList();

                if (available.Count > 0)
                {
                    results.Add(new DoctorWithSlotsViewModel
                    {
                        DoctorId = doctor.DoctorId,
                        FullName = fullName,
                        Specialization = specialization,
                        FreeSlots = available
                    });
                }
            }

            return results;
        }

        private List<TimeOnly> GenerateTimeSlots(TimeOnly from, TimeOnly to, TimeSpan interval)
        {
            var result = new List<TimeOnly>();
            var time = from;
            while (time < to)
            {
                result.Add(time);
                time = time.Add(interval);
            }
            return result;
        }
    }
}
