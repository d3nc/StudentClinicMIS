﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views.Registrar;
using System;
using System.Windows;
using System.Windows.Input;
using StudentClinicMIS.Views.DoctorModels;
using StudentClinicMIS.Views.Admin;

namespace StudentClinicMIS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IServiceScope _scope;
        private readonly PolyclinicContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private bool _isProcessingLogin = false;

        public LoginWindow()
        {
            InitializeComponent();
            _scope = App.AppHost.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<PolyclinicContext>();
            _passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessingLogin) return;
            _isProcessingLogin = true;

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                _isProcessingLogin = false;
                return;
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null || !_passwordHasher.Verify(password, user.Password))
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _isProcessingLogin = false;
                    return;
                }

                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                Window mainWindow = null;

                if (user.Role?.ToLower() == "doctor")
                {
                    var employee = await _context.Employees
                        .Include(e => e.Doctors)
                        .FirstOrDefaultAsync(e => e.UserId == user.UserId);

                    if (employee == null || employee.Doctors.Count == 0)
                    {
                        MessageBox.Show("Пользователь не связан с врачом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        _isProcessingLogin = false;
                        return;
                    }

                    var doctorId = employee.Doctors.First().DoctorId;
                    mainWindow = new DoctorMainWindow(doctorId);
                }
                else
                {
                    mainWindow = user.Role?.ToLower() switch
                    {
                        "admin" => App.AppHost.Services.GetService<AdminMainWindow>(),
                        "receptionist" => App.AppHost.Services.GetService<RegistrarMainWindow>(),
                        _ => null
                    };
                }

                if (mainWindow == null)
                {
                    MessageBox.Show($"Неизвестная роль: {user.Role}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _isProcessingLogin = false;
                    return;
                }

                this.Hide();
                mainWindow.Closed += (s, args) => this.Close();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessingLogin = false;
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
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
