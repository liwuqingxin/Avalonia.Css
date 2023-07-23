using System;
using System.Security.Cryptography;
using Avalonia.Animation.Easings;
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
                return duration;
            }

            typeof(DataParser).WriteError($"Can not parse TimeSpan from string '{value}'.");
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
                typeof(DataParser).WriteError($"Can not parse Color from string '{colorString}'.");
                return null;
            }
        }

        public static Easing? TryParseEasing(string easingString)
        {
            try
            {
                return Easing.Parse(easingString);
            }
            catch
            {
                typeof(DataParser).WriteError($"Can not parse Easing from string '{easingString}'.");
                return null;
            }
        }
    }
}
