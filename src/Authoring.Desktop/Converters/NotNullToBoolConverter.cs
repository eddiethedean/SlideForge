using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Authoring.Desktop.Converters;

public class NotNullToBoolConverter : IValueConverter
{
    public static readonly NotNullToBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isNotNull = value != null;
        if (parameter?.ToString() == "inverse")
        {
            return !isNotNull;
        }
        return isNotNull;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
