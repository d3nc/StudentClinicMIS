using System;
using System.Globalization;
using System.Windows.Data;
using StudentClinicMIS.Models;

namespace StudentClinicMIS.Converters
{
    public class EmployeeFullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Employee employee)
            {
                return $"{employee.LastName} {employee.FirstName} {employee.MiddleName}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}