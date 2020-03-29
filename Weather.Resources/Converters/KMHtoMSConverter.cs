using System;
using System.Globalization;
using System.Windows.Data;

namespace Weather.Resources.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class KMHtoMSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value)*1000/3600;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
