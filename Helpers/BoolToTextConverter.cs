using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiFirebase.Helpers
{
    public class BoolToTextConverter : IValueConverter
    {
        // Usa parámetros como "Editar;Registrar"
        object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolVal && parameter is string param)
            {
                var parts = param.Split(';');
                if (parts.Length == 2)
                {
                    return boolVal ? parts[0] : parts[1];
                }
            }

            return value?.ToString() ?? string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                // Convertir el valor booleano a texto
                return booleanValue ? "True" : "False";
            }
            return null; // Retornar null si el valor no es un booleano
        }
    }
}
