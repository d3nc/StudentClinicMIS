using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using System;
using System.Windows;

namespace StudentClinicMIS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly PolyclinicContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public LoginWindow()
        {
            InitializeComponent();

            var scope = App.AppHost.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<PolyclinicContext>();
            _passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);

                if (user == null || !_passwordHasher.Verify(password, user.Password))
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                user.LastLogin = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                Window mainWindow = user.Role.ToLower() switch
                {
                    "admin" => App.AppHost.Services.GetRequiredService<AdminMainWindow>(),
                    "doctor" => App.AppHost.Services.GetRequiredService<DoctorMainWindow>(),
                    "receptionist" => App.AppHost.Services.GetRequiredService<RegistrarMainWindow>(),
                    _ => null
                };

                if (mainWindow == null)
                {
                    MessageBox.Show($"Неизвестная роль: {user.Role}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
