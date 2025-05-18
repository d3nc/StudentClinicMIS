using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PolyclinicContext _context;

        public PatientRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.Gender)
                .Include(p => p.StudentCard)
                    .ThenInclude(sc => sc.Group)
                        .ThenInclude(g => g.Faculty)
                .OrderBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Gender)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task AddAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

    }
}