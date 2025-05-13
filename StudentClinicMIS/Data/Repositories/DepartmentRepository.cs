using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly PolyclinicContext _context;

        public DepartmentRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .Include(d => d.HeadDoctor)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int departmentId)
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .Include(d => d.HeadDoctor)
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);
        }

        public async Task AddAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }
    }
}
