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

            // ✅ Colores más suaves
            Color verdeSuave = Color.FromArgb("#02f765"); // verde menta claro
            Color rojoSuave = Color.FromArgb("#ff0d65");  // rojo coral claro

            return estado ? verdeSuave : rojoSuave;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
