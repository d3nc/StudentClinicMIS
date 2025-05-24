using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Data;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly PolyclinicContext _context;

        public SpecializationRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Specialization>> GetAllAsync()
        {
            return await _context.Specializations
                .Include(s => s.Doctors)
                .ToListAsync();
        }

        public async Task<Specialization?> GetByIdAsync(int id)
        {
            return await _context.Specializations
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.SpecializationId == id);
        }

        public async Task AddAsync(Specialization specialization)
        {
            await _context.Specializations.AddAsync(specialization);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Specialization specialization)
        {
            _context.Specializations.Update(specialization);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var specialization = await GetByIdAsync(id);
            if (specialization != null)
            {
                _context.Specializations.Remove(specialization);
                await _context.SaveChangesAsync();
            }
        }
    }
}