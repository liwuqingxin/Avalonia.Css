using System;
using System.Reflection;
using System.Security.Cryptography;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css
{
    internal static class DataParser
    {
        public static TimeSpan TryParseTimeSpan(string value)
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

        public static Color ApplyOpacity(this Color c, double opacity)
        {
            return new Color((byte)(c.A * opacity), c.R, c.G, c.B);
        }

        public static void ApplyVarForDuration(this ITransition transition, IAcssBuilder acssBuilder, string? key)
        {
            if (key == null || !acssBuilder.ResourceProvidersManager.TryFindResource(key, out var resource))
            {
                return;
            }
            
            var prop = transition.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public);
            switch (resource)
            {
                case double d:
                    prop?.SetValue(transition, TimeSpan.FromSeconds(d));
                    break;
                case TimeSpan t:
                    prop?.SetValue(transition, t);
                    break;
            }
        }
        
        public static void ApplyVarForDelay(this ITransition transition, IAcssBuilder acssBuilder, string? key)
        {
            if (key == null || !acssBuilder.ResourceProvidersManager.TryFindResource(key, out var resource))
            {
                return;
            }
            
            var prop = transition.GetType().GetProperty("Delay", BindingFlags.Instance | BindingFlags.Public);
            switch (resource)
            {
                case double d:
                    prop?.SetValue(transition, TimeSpan.FromSeconds(d));
                    break;
                case TimeSpan t:
                    prop?.SetValue(transition, t);
                    break;
            }
        }
        
        public static void ApplyVarForEasing(this ITransition transition, IAcssBuilder acssBuilder, string? key)
        {
            if (key == null || !acssBuilder.ResourceProvidersManager.TryFindResource(key, out var resource))
            {
                return;
            }
            
            var prop = transition.GetType().GetProperty("Easing", BindingFlags.Instance | BindingFlags.Public);
            prop?.SetValue(transition, resource);
        }
    }
}
