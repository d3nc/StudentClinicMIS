using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.ViewModels.Admin;
using System.Windows.Controls;

namespace StudentClinicMIS.Views.Admin
{
    public partial class UsersPage : UserControl
    {
        public UsersPage()
        {
            InitializeComponent();
            var viewModel = App.AppHost.Services.GetRequiredService<UsersPageViewModel>();
            DataContext = viewModel;
        }
    }
}
