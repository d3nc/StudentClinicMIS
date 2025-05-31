using StudentClinicMIS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClinicMIS.Data.Interfaces
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetByDepartmentIdAsync(int departmentId);
        Task<DoctorScheduleEntity> GetScheduleForDoctorAsync(int doctorId, DayOfWeek dayOfWeek);
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
    }
}