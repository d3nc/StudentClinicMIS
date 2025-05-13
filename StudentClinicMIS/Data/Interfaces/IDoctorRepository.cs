using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Interfaces
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetByDepartmentIdAsync(int departmentId);
    }
}
