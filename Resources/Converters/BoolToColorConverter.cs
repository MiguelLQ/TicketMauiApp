using System.Globalization;

namespace MauiFirebase.Resources.Converters;
public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isEnabled)
        {
            if (isEnabled)
            {
                return Color.FromRgb(0, 0, 139);
            }
            else
            {
                return Color.FromRgba(0, 0, 139, 100);
            }
        }

        throw new ArgumentException("sin valor", nameof(value));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
