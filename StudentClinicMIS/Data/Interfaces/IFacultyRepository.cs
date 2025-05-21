using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Interfaces
{
    // IFacultyRepository.cs
    public interface IFacultyRepository
    {
        Task<List<Faculty>> GetAllAsync();
        Task<Faculty?> GetByIdAsync(int id);
    }
}
