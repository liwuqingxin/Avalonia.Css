using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.Css.App
{
    public class GDPValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int gdp)
            {
                return gdp switch
                {
                    <= 5000  => new SolidColorBrush(Colors.Orange,     0.6),
                    <= 10000 => new SolidColorBrush(Colors.Yellow,     0.6),
                    _        => new SolidColorBrush(Colors.LightGreen, 0.6)
                };
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}