using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views.Registrar;
using System;
using System.Windows;
using System.Windows.Input;

namespace StudentClinicMIS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IServiceScope _scope;
        private readonly PolyclinicContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public LoginWindow()
        {
            InitializeComponent();

            // Создание области зависимостей
            _scope = App.AppHost.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<PolyclinicContext>();
            _passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
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
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);

                if (user == null || !_passwordHasher.Verify(password, user.Password))
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                Window? mainWindow = user.Role?.ToLower() switch
                {
                    "admin" => App.AppHost.Services.GetService<AdminMainWindow>(),
                    "doctor" => App.AppHost.Services.GetService<DoctorMainWindow>(),
                    "receptionist" => App.AppHost.Services.GetService<RegistrarMainWindow>(),
                    _ => null
                };

                if (mainWindow == null)
                {
                    MessageBox.Show($"Неизвестная роль: {user.Role}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                mainWindow.Show();
                Close(); // Закрытие окна входа
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            _scope?.Dispose();
            base.OnClosed(e);
        }
    }
}
