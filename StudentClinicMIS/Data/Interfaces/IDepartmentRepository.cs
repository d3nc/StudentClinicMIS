using System.Collections.Generic;
using System.Threading.Tasks;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Data.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int departmentId);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int departmentId);
    }
}
