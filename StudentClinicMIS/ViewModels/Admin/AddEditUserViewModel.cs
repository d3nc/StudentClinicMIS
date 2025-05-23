using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentClinicMIS.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace StudentClinicMIS.ViewModels.Admin;

public partial class AddEditUserViewModel : ObservableObject
{
    public AddEditUserViewModel(User user)
    {
        CurrentUser = user;
        Username = user.Username;
        Password = user.Password;
        Role = user.Role;

        Roles = new ObservableCollection<string> { "admin", "registrar", "doctor" };
    }

    public User CurrentUser { get; }

    [ObservableProperty] private string username;
    [ObservableProperty] private string password;
    [ObservableProperty] private string role;
    public ObservableCollection<string> Roles { get; }

    [RelayCommand]
    public void Save(Window window)
    {
        CurrentUser.Username = Username;
        CurrentUser.Password = Password;
        CurrentUser.Role = Role;

        window.DialogResult = true;
        window.Close();
    }
}
