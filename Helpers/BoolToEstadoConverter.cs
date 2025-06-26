using System.Globalization;
namespace MauiFirebase.Helpers;

public class BoolToEstadoConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value ? "Activo" : "Inactivo";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (string)value == "Activo";
    }
}
