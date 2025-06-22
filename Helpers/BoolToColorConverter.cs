using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiFirebase.Helpers
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool estado = value is bool b && b;
            return estado ? Colors.Green : Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
