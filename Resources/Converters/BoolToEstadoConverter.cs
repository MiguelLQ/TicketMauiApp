
using System.Globalization;

namespace MauiFirebase.Resources.Converters;
public class BoolToEstadoConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool estado)
        {
            return estado ? "Activo" : "Inactivo";
        }
        return "Desconocido";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
