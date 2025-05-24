using System.Windows;
using StudentClinicMIS.Views.Admin;

namespace StudentClinicMIS.Views.Admin
{
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
            MainContent.Content = new UsersPage(); // стартовая страница
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UsersPage(); // переключение вручную
        }
        private void DoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DoctorsPage();
        }
    }
}
