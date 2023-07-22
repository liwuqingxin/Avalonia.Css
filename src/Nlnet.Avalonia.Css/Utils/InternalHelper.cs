using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal static class InternalHelper
    {
        private static PropertyInfo? GetPropertyInfo<T>(string name) where T : class
        {
            return typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private static MethodInfo? GetMethodInfo<T>(string name) where T : class
        {
            return typeof(T).GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private static PropertyInfo? SelectorTargetTypePropertyInfo { get; } = GetPropertyInfo<Selector>("TargetType");

        private static MethodInfo? StyledElementInvalidateStylesMethodInfo { get; } = GetMethodInfo<StyledElement>("InvalidateStyles");

        private static MethodInfo? StyledElementOnControlThemeChangedMethodInfo { get; } = GetMethodInfo<StyledElement>("OnControlThemeChanged");

        private static MethodInfo? StyledElementOnTemplatedParentControlThemeChangedMethodInfo { get; } = GetMethodInfo<StyledElement>("OnTemplatedParentControlThemeChanged");

        public static Type? GetTargetType(this Selector selector)
        {
            if (SelectorTargetTypePropertyInfo == null)
            {
                throw new NotSupportedException($"Can not find the 'TargetType' property in the type of '{nameof(Selector)}'.");
            }

            var targetType = SelectorTargetTypePropertyInfo.GetValue(selector) as Type;
            return targetType;
        }

        public static void InvalidStyles(this StyledElement element)
        {
            if (StyledElementInvalidateStylesMethodInfo == null)
            {
                throw new NotSupportedException($"Can not find the InvalidateStylesMethodInfo in the type of '{nameof(StyledElement)}'.");
            }

            StyledElementInvalidateStylesMethodInfo?.Invoke(element, new object[] { true });
        }

        public static void OnControlThemeChanged(this StyledElement element)
        {
            if (StyledElementOnControlThemeChangedMethodInfo == null)
            {
                throw new NotSupportedException($"Can not find the OnControlThemeChangedMethodInfo in the type of '{nameof(StyledElement)}'.");
            }

            StyledElementOnControlThemeChangedMethodInfo?.Invoke(element, null);
        }

        public static void OnTemplatedParentControlThemeChanged(this StyledElement element)
        {
            if (StyledElementOnTemplatedParentControlThemeChangedMethodInfo == null)
            {
                throw new NotSupportedException($"Can not find the OnTemplatedParentControlThemeChangedMethodInfo in the type of '{nameof(StyledElement)}'.");
            }

            StyledElementOnTemplatedParentControlThemeChangedMethodInfo?.Invoke(element, null);
        }
    }
}
