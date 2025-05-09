using System.Collections.Generic;
using System.Threading.Tasks;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Data.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetByPatientIdAsync(int patientId);
    }
}
