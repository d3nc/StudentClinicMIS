using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace YourNamespace
{
    public partial class AppointmentPanel : UserControl
    {
        public AppointmentPanel()
        {
            InitializeComponent();
        }

        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (PatientComboBox.SelectedItem == null ||
                DepartmentComboBox.SelectedItem == null ||
                DoctorComboBox.SelectedItem == null ||
                DatePicker.SelectedDate == null)
            {
                ShowErrorMessage("Пожалуйста, заполните все поля.");
                return;
            }

            // Здесь можно добавить сохранение данных

            ShowSuccessMessage("Запись успешно сохранена!");
        }

        private void ShowErrorMessage(string message)
        {
            var snackbar = FindVisualChild<Snackbar>(this);
            snackbar?.MessageQueue?.Enqueue(message);
        }

        private void ShowSuccessMessage(string message)
        {
            var snackbar = FindVisualChild<Snackbar>(this);
            snackbar?.MessageQueue?.Enqueue(message);
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                {
                    return t;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
