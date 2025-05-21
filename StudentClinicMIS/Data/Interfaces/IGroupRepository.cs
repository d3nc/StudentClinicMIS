using StudentClinicMIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<StudentGroup>> GetByFacultyIdAsync(int facultyId);
    }
}
