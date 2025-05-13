using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DoctorRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<Doctor>> GetByDepartmentIdAsync(int departmentId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            return await context.Doctors
                .Include(d => d.Employee)
                .Include(d => d.Specialization) 
                .Where(d => d.Employee.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<DoctorScheduleEntity?> GetScheduleForDoctorAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();

            return await context.DoctorSchedules1
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s =>
                    s.DoctorId == doctorId &&
                    s.DayOfWeek.ToLower() == dayOfWeek.ToString().ToLower());
        }
    }
}
