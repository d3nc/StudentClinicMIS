using System;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;

namespace StudentClinicMIS.Converters
{
    public class DateToSlotStatusConverter : IValueConverter
    {
        public HashSet<DateOnly> BusyDates { get; set; } = new();
        public HashSet<DateOnly> FreeDates { get; set; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                var date = DateOnly.FromDateTime(dt);
                if (BusyDates.Contains(date)) return "Busy";
                if (FreeDates.Contains(date)) return "Free";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
