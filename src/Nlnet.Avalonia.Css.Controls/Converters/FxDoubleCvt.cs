using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Nlnet.Avalonia.Css.Controls;

/// <summary>
/// A converter to calculate '<see langword="f(x) = Factor * x^Order + Offset"/>', where x is a double.
/// </summary>
public class FxDoubleCvt : IValueConverter
{
    public double Factor { get; set; } = 1d;

    public double Offset { get; set; } = 0d;

    public double Order { get; set; } = 1d;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return Factor * Math.Pow(d, Order) + Offset;
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}