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

namespace StudentClinicMIS
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
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

                    // Репозитории
                    services.AddScoped<IPatientRepository, PatientRepository>();
                    services.AddScoped<IAppointmentRepository, AppointmentRepository>();

                    // Авторизация
                    services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

                    // Окна по ролям
                    services.AddTransient<AdminMainWindow>();
                    services.AddTransient<DoctorMainWindow>();
                    services.AddTransient<RegistrarMainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();
            base.OnStartup(e);

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }
    }
}
