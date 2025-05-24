using System.Collections.ObjectModel;
using System.Windows;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Views.Admin
{
    public partial class AddEditDoctorWindow : Window
    {
        public Doctor Doctor { get; private set; }
        public ObservableCollection<Specialization> Specializations { get; }
        public ObservableCollection<Department> Departments { get; }

        public AddEditDoctorWindow(Doctor doctor, ObservableCollection<Specialization> specializations, ObservableCollection<Department> departments)
        {
            InitializeComponent();

            Doctor = doctor;
            Specializations = specializations;
            Departments = departments;

            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
