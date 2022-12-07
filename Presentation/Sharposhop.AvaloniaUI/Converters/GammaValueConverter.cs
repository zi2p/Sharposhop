using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.Converters;

public class GammaValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Gamma model)
            throw new NotSupportedException("Value must be of type GammaModel");

        return model.Value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double d)
            throw new NotSupportedException("Value must be of type double");

        return new Gamma((float)d);
    }
}