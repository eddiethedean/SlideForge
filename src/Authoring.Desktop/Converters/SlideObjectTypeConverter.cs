using Avalonia.Data.Converters;
using Authoring.Core.Models;
using System;
using System.Globalization;

namespace Authoring.Desktop.Converters;

public class SlideObjectTypeConverter : IValueConverter
{
    public static readonly SlideObjectTypeConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SlideObject obj || parameter is not string typeName)
            return false;

        return typeName switch
        {
            "Text" => obj is TextObject,
            "Image" => obj is ImageObject,
            "Button" => obj is ButtonObject,
            _ => false
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
