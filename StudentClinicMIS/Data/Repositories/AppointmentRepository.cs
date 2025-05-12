using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PolyclinicContext _context;

        public AppointmentRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Employee)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .ToListAsync();
        }
        public async Task AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .Include(d => d.Employee)
                .ToListAsync();
        }

    }
}
