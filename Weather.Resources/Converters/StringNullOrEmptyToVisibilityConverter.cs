using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace Weather.Resources.Converters
{

    public class StringNullOrEmptyToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString();
            return string.IsNullOrEmpty(val)
                ? Visibility.Hidden : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
