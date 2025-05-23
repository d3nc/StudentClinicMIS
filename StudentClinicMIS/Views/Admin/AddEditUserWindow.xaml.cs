using StudentClinicMIS.Models;
using System.Windows;

namespace StudentClinicMIS.Views.Admin
{
    public partial class AddEditUserWindow : Window
    {
        public User User { get; private set; }
        public string? Password { get; private set; }

        public AddEditUserWindow()
        {
            InitializeComponent();
            User = new User();
            DataContext = this;
        }

        public AddEditUserWindow(User user)
        {
            InitializeComponent();
            User = user;
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Password = PasswordBox.Password;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
