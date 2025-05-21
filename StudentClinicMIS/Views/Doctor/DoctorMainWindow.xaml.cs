using System.Windows;
using StudentClinicMIS.Models;
using StudentClinicMIS.ViewModels;
using StudentClinicMIS.ViewModels.Doctor;

namespace StudentClinicMIS.Views
{
    public partial class DoctorMainWindow : Window
    {
        public DoctorMainWindow(Doctor doctor)
        {
            InitializeComponent();
            DataContext = new DoctorMainViewModel(doctor);
        }
    }
}