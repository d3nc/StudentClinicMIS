using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace StudentClinicMIS.Views.Admin
{
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
            LoadInitialPage();
            StateChanged += OnWindowStateChanged;
        }

        private void LoadInitialPage()
        {
            UsersButton.Background = new SolidColorBrush(Color.FromRgb(0x67, 0x3A, 0xB7));
            LoadPage(new UsersPage());
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            MaximizeIcon.Kind = WindowState == WindowState.Maximized 
                ? PackIconKind.WindowRestore 
                : PackIconKind.WindowMaximize;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    MaximizeButton_Click(null, null);
                }
                else
                {
                    DragMove();
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMenuButtons();
            UsersButton.Background = new SolidColorBrush(Color.FromRgb(0x67, 0x3A, 0xB7));
            LoadPage(new UsersPage());
        }

        private void DoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMenuButtons();
            DoctorsButton.Background = new SolidColorBrush(Color.FromRgb(0x67, 0x3A, 0xB7));
            LoadPage(new DoctorsPage());
        }

        private void ResetMenuButtons()
        {
            UsersButton.Background = Brushes.Transparent;
            DoctorsButton.Background = Brushes.Transparent;
        }

        private void LoadPage(UserControl page)
        {
            MainContent.Content = page;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            StateChanged -= OnWindowStateChanged;
            base.OnClosed(e);
        }
    }
}