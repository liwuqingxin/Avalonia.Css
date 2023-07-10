using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Nlnet.Avalonia.Css.Controls;

public class SliderValueFormatCvt : IMultiValueConverter
{
    public static SliderValueFormatCvt Cvt { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count > 0 && values[0] is Slider slider)
        {
            var format = SliderExtension.GetFormat(slider) ?? "{0:F1}";
            return string.Format(format, slider.Value);
        }

        return BindingOperations.DoNothing;
    }
}
