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

            if (Doctor.Employee == null)
                Doctor.Employee = new Employee();

            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Doctor.FullName))
            {
                MessageBox.Show("Введите ФИО врача.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Doctor.Specialization == null)
            {
                MessageBox.Show("Выберите специальность.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Doctor.Employee?.DepartmentId == null || Doctor.Employee.DepartmentId == 0)
            {
                MessageBox.Show("Выберите отделение.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
