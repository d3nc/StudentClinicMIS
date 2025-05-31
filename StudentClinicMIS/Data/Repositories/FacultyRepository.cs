using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Data.Repositories
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly PolyclinicContext _context;

        public FacultyRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task<List<Faculty>> GetAllAsync()
        {
            return await _context.Faculties.ToListAsync();
        }

        public async Task<Faculty?> GetByIdAsync(int id)
        {
            return await _context.Faculties.FindAsync(id);
        }
    }
}
