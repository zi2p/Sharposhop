using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Sharposhop.Core.Model;

namespace Sharposhop.AvaloniaUI.Converters;

public class DeNormalizedFractionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Fraction fraction)
            throw new NotSupportedException("Value must be a fraction");

        return fraction.Value * 255d;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double fraction)
            throw new NotSupportedException("Value must be a float");

        return new Fraction((float)fraction / 255);
    }
}