using StudentClinicMIS.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly PolyclinicContext _context;

        public GroupRepository(PolyclinicContext context)
        {
            _context = context;
        }

        public async Task<List<StudentGroup>> GetByFacultyIdAsync(int facultyId)
        {
            return await _context.StudentGroups
                .Where(g => g.FacultyId == facultyId)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
    }
}
