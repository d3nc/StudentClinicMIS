using System.Collections.Generic;
using System.Windows;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Views
{
    public partial class PatientAppointmentsWindow : Window
    {
        public PatientAppointmentsWindow(List<Appointment> appointments)
        {
            InitializeComponent();
            AppointmentsDataGrid.ItemsSource = appointments;
        }
    }
}
