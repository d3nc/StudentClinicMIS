using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.ViewModels.Doctor;

namespace StudentClinicMIS.Views.DoctorModels
{
    public partial class DoctorMainWindow : Window
    {
        private readonly int _doctorId;

        public DoctorMainWindow(int doctorId)
        {
            InitializeComponent();
            _doctorId = doctorId;
            LoadPage("SchedulePage");
        }

        private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationList.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag is string pageKey)
            {
                LoadPage(pageKey);
            }
        }

        private void LoadPage(string pageKey)
        {
            switch (pageKey)
            {
                case "SchedulePage":
                    var viewModelFactory = App.AppHost.Services.GetRequiredService<ISchedulePageViewModelFactory>();
                    var schedulePageViewModel = viewModelFactory.Create(_doctorId);
                    var schedulePage = new SchedulePage(schedulePageViewModel);
                    MainFrame.Navigate(schedulePage);
                    break;

                case "PatientsPage":
                    MainFrame.Navigate(new Page
                    {
                        Content = new TextBlock
                        {
                            Text = "Пациенты (в разработке)",
                            FontSize = 20,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                    break;

                case "RecordsPage":
                    MainFrame.Navigate(new Page
                    {
                        Content = new TextBlock
                        {
                            Text = "История приёмов (в разработке)",
                            FontSize = 20,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                    break;

                case "PrescriptionsPage":
                    MainFrame.Navigate(new Page
                    {
                        Content = new TextBlock
                        {
                            Text = "Рецепты (в разработке)",
                            FontSize = 20,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                    break;

                case "DiagnosticsPage":
                    MainFrame.Navigate(new Page
                    {
                        Content = new TextBlock
                        {
                            Text = "Диагностика (в разработке)",
                            FontSize = 20,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                    break;
            }
        }
    }
}
