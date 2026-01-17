using Avalonia.Data.Converters;
using Authoring.Core.Models;
using System;
using System.Globalization;

namespace Authoring.Desktop.Converters;

public class ActionTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ActionType actionType)
        {
            return actionType switch
            {
                ActionType.NavigateToSlide => "Navigate to Slide",
                ActionType.SetVariable => "Set Variable",
                ActionType.ShowLayer => "Show Layer",
                ActionType.HideLayer => "Hide Layer",
                _ => actionType.ToString()
            };
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
