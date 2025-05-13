using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StudentClinicMIS.Converters
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush OccupiedBrush { get; set; } = Brushes.Red;
        public Brush AvailableBrush { get; set; } = Brushes.Green;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool occupied)
            {
                return occupied ? OccupiedBrush : AvailableBrush;
            }
            return AvailableBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
