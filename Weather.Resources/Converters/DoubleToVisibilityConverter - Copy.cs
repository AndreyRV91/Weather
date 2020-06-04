using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace Weather.Resources.Converters
{

    public class DoubleToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value == default? Visibility.Hidden : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
