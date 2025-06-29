using System.Globalization;

namespace MauiFirebase.Resources.Converters;

public class StringToIntConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var str = value as string;

        if (string.IsNullOrWhiteSpace(str))
        {

            return null; 
        }

        if (int.TryParse(str, out int result))
        {
            return result;
        }

        return null;
    }
}
