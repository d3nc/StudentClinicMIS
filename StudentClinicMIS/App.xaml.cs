using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Data.Repositories;
using StudentClinicMIS.Data.Services;
using StudentClinicMIS.Views;
using StudentClinicMIS.Views.Registrar;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using StudentClinicMIS.ViewModels.Registrar;
using StudentClinicMIS.ViewModels.Doctor;
using StudentClinicMIS.Views.DoctorModels;
using StudentClinicMIS.Views.Admin;
using StudentClinicMIS.ViewModels.Admin;
namespace StudentClinicMIS
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {

            // 2. Затем настраиваем хост
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<PolyclinicContext>(options =>
                        options.UseNpgsql(connectionString));


                    services.AddScoped<IPatientRepository, PatientRepository>();
                    services.AddScoped<IAppointmentRepository, AppointmentRepository>();
                    services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
                    services.AddScoped<IDoctorRepository, DoctorRepository>();
                    services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                    services.AddScoped<Doctor>();
                    services.AddScoped<AvailableDoctorsPanelViewModel>();
                    services.AddScoped<IEmployeeRepository, EmployeeRepository>();
                    services.AddScoped<ISpecializationRepository, SpecializationRepository>();

                    services.AddTransient<AdminMainWindow>();
                    services.AddTransient<SchedulePageViewModel>();
                    services.AddTransient<SchedulePage>();
                    services.AddTransient<UsersPageViewModel>();
                    services.AddTransient<LoginWindow>();
                    services.AddScoped<IFacultyRepository, FacultyRepository>();
                    services.AddTransient<RegistrarMainWindow>();
                    services.AddTransient<DoctorMainWindow>();
                    services.AddTransient<RegistrarMainWindow>();
                    services.AddScoped<IGroupRepository, GroupRepository>();
                    services.AddTransient<DoctorsPageViewModel>();
                    services.AddTransient<AddEditDoctorViewModel>();
                    services.AddScoped<ISchedulePageViewModelFactory, SchedulePageViewModelFactory>();

                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppHost.StartAsync();


                var loginWindow = AppHost.Services.GetRequiredService<LoginWindow>();
                loginWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (AppHost)
            {
                await AppHost.StopAsync();
            }
            base.OnExit(e);
        }
    }
}
