using System;
using System.Reflection;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal static class InternalHelper
    {
        private static PropertyInfo? SelectorTargetTypePropertyInfo { get; } = typeof(Selector).GetProperty("TargetType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static Type? GetTargetType(this Selector selector)
        {
            if (SelectorTargetTypePropertyInfo == null)
            {
                throw new NotSupportedException($"Can not find the 'TargetType' property in the type of '{nameof(Selector)}'.");
            }

            var targetType = SelectorTargetTypePropertyInfo.GetValue(selector) as Type;
            return targetType;
        }
    }
}
