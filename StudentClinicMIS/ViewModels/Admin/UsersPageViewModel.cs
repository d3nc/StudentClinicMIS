using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudentClinicMIS.Data;
using StudentClinicMIS.Data.Interfaces;
using StudentClinicMIS.Models;
using StudentClinicMIS.Views.Admin;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace StudentClinicMIS.ViewModels.Admin
{
    public partial class UsersPageViewModel : ObservableObject
    {
        private readonly PolyclinicContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UsersPageViewModel(PolyclinicContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            Users = new ObservableCollection<User>();
            LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
            AddUserCommand = new AsyncRelayCommand(AddUserAsync);
            EditUserCommand = new AsyncRelayCommand(EditUserAsync, CanEditOrDelete);
            DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync, CanEditOrDelete);
            LoadUsersCommand.Execute(null);
        }

        public ObservableCollection<User> Users { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditUserCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteUserCommand))]
        private User? selectedUser;

        public IAsyncRelayCommand LoadUsersCommand { get; }
        public IAsyncRelayCommand AddUserCommand { get; }
        public IAsyncRelayCommand EditUserCommand { get; }
        public IAsyncRelayCommand DeleteUserCommand { get; }

        private async Task LoadUsersAsync()
        {
            var usersFromDb = await _context.Users.ToListAsync();
            Users.Clear();
            foreach (var user in usersFromDb)
            {
                Users.Add(user);
            }
        }

        private async Task AddUserAsync()
        {
            var dialog = new AddEditUserWindow();
            if (dialog.ShowDialog() == true)
            {
                var newUser = dialog.User;

                // 🔍 Проверка на уникальность логина (без учёта регистра)
                bool usernameExists = await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == newUser.Username.ToLower());

                if (usernameExists)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(dialog.Password))
                {
                    newUser.Password = _passwordHasher.Hash(dialog.Password);
                }
                else
                {
                    MessageBox.Show("Пароль не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                Users.Add(newUser);
            }
        }

        private async Task EditUserAsync()
        {
            if (SelectedUser is null) return;

            var dialog = new AddEditUserWindow(SelectedUser);
            if (dialog.ShowDialog() == true)
            {
                var updatedUser = dialog.User;

                if (!string.IsNullOrWhiteSpace(dialog.Password))
                {
                    updatedUser.Password = _passwordHasher.Hash(dialog.Password);
                }

                _context.Entry(updatedUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await LoadUsersAsync();
            }
        }

        private async Task DeleteUserAsync()
        {
            if (SelectedUser is null) return;

            var result = MessageBox.Show($"Удалить пользователя {SelectedUser.Username}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _context.Users.Remove(SelectedUser);
                await _context.SaveChangesAsync();
                Users.Remove(SelectedUser);
            }
        }

        private bool CanEditOrDelete() => SelectedUser is not null;
    }
}
