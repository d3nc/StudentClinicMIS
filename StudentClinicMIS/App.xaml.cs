using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.IO;
using StudentClinicMIS;
using Microsoft.EntityFrameworkCore;
using System;
using StudentClinicMIS.Models;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Data.Repositories;

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

                    services.AddScoped<IPatientRepository, PatientRepository>();
                    services.AddTransient<MainWindow>();
                    services.AddScoped<IAppointmentRepository, AppointmentRepository>();

                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();
            base.OnStartup(e);

            var scope = AppHost.Services.CreateScope();
            var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }
    }
}