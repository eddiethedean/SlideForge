using Avalonia.Data.Converters;
using Authoring.Core.Models;
using System;
using System.Globalization;

namespace Authoring.Desktop.Converters;

public class TriggerTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TriggerType triggerType)
        {
            return triggerType switch
            {
                TriggerType.OnClick => "On Click",
                TriggerType.OnTimelineStart => "On Timeline Start",
                _ => triggerType.ToString()
            };
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
