using System;
using System.Globalization;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css
{
    // TODO Unit test.
    internal static class SimpleDataParser
    {
        public static TimeSpan? TryParseTimeSpan(this string value)
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

            typeof(SimpleDataParser).WriteError($"Can not parse {nameof(TimeSpan)} from string '{value}'.");
            return null;
        }

        public static Color? TryParseColor(this string value)
        {
            if (Color.TryParse(value, out var c))
            {
                return c;
            }

            typeof(SimpleDataParser).WriteError($"Can not parse {nameof(Color)} from string '{value}'.");
            return null;
        }

        public static Easing? TryParseEasing(this string value)
        {
            try
            {
                return Easing.Parse(value);
            }
            catch
            {
                typeof(SimpleDataParser).WriteError($"Can not parse {nameof(Easing)} from string '{value}'.");
                return null;
            }
        }

        public static BoxShadows? TryParseBoxShadow(this string value)
        {
            try
            {
                return BoxShadows.Parse(value);
            }
            catch
            {
                typeof(SimpleDataParser).WriteError($"Can not parse {nameof(BoxShadows)} from string '{value}'.");
                return null;
            }
        }

        public static double? TryParseDouble(this string value)
        {
            if (double.TryParse(value, out var d))
            {
                return d;
            }

            typeof(SimpleDataParser).WriteError($"Can not parse {nameof(Double)} from string '{value}'.");
            return null;
        }

        public static int? TryParseInt(this string value)
        {
            if (int.TryParse(value, out var i))
            {
                return i;
            }

            typeof(SimpleDataParser).WriteError($"Can not parse int from string '{value}'.");
            return null;
        }

        public static Thickness? TryParseThickness(this string value)
        {
            try
            {
                return Thickness.Parse(value);
            }
            catch
            {
                typeof(SimpleDataParser).WriteError($"Can not parse {nameof(Thickness)} from string '{value}'.");
                return null;
            }
        }

        public static KeySpline? TryParseKeySpline(this string value)
        {
            try
            {
                return KeySpline.Parse(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                typeof(SimpleDataParser).WriteError($"Can not parse KeySpline from string '{value}'.");
                return null;
            }
        }


        public static Color ApplyOpacity(this Color c, double opacity)
        {
            return new Color((byte)(c.A * opacity), c.R, c.G, c.B);
        }
    }
}
