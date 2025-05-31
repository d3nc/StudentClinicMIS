using System.Windows.Controls;
using StudentClinicMIS.ViewModels.Doctor;

namespace StudentClinicMIS.Views.DoctorModels
{
    public partial class SchedulePage : Page
    {
        public SchedulePage(SchedulePageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
