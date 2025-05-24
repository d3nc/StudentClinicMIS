using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.ViewModels.Admin;
using System.Windows.Controls;

namespace StudentClinicMIS.Views.Admin
{
    public partial class DoctorsPage : UserControl
    {
        public DoctorsPage()
        {
            InitializeComponent();
            DataContext = App.AppHost.Services.GetRequiredService<DoctorsPageViewModel>();
        }
    }
}