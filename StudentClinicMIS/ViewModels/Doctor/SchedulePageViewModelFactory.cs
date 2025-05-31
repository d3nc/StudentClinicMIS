using StudentClinicMIS.Models;

namespace StudentClinicMIS.ViewModels.Doctor
{
    public interface ISchedulePageViewModelFactory
    {
        SchedulePageViewModel Create(int doctorId);
    }

    public class SchedulePageViewModelFactory : ISchedulePageViewModelFactory
    {
        private readonly PolyclinicContext _context;

        public SchedulePageViewModelFactory(PolyclinicContext context)
        {
            _context = context;
        }

        public SchedulePageViewModel Create(int doctorId)
        {
            return new SchedulePageViewModel(_context, doctorId);
        }
    }
}
