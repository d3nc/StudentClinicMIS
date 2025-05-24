using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Data;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly PolyclinicContext _context;

        public DoctorRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await GetByIdAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Employee)
                .Include(d => d.Specialization)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Employee)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<List<Doctor>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Doctors
                .Where(d => d.Employee.DepartmentId == departmentId)
                .Include(d => d.Employee)
                .Include(d => d.Specialization)
                .ToListAsync();
        }

        public async Task<DoctorScheduleEntity> GetScheduleForDoctorAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            return await _context.DoctorSchedules1
                .Include(ds => ds.Doctor)
                .Include(ds => ds.Room)
                .FirstOrDefaultAsync(ds => ds.DoctorId == doctorId &&
                                       ds.DayOfWeek == dayOfWeek.ToString());
        }
    }
}