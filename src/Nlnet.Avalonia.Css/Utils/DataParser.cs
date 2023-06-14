using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css
{
    internal static class DataParser
    {
        public static TimeSpan ParseTimeSpan(string value)
        {
            if (TimeSpan.TryParse(value, out var duration))
            {
                return duration;
            }

            if (double.TryParse(value, out var milliseconds))
            {
                duration = TimeSpan.FromSeconds(milliseconds);
            }

            return duration;
        }

        public static Color? TryParseColor(string colorString)
        {
            try
            {
                return Color.Parse(colorString);
            }
            catch
            {
                return null;
            }
        }
    }
}
