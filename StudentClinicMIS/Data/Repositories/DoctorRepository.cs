using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Doctor>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Doctors
                .Include(d => d.Employee)
                .Where(d => d.Employee.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}
