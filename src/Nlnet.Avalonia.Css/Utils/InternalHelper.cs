using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal static class InternalHelper
    {
        private static readonly PropertyInfo? SelectorTargetTypePi = GetPropertyInfo<Selector>("TargetType");
        private static readonly MethodInfo? DetachStylesMi = GetMethodInfo<StyledElement>("DetachStyles");
        private static readonly MethodInfo? StyledElementInvalidateStylesMi = GetMethodInfo<StyledElement>("InvalidateStyles");
        private static readonly MethodInfo? StyledElementOnControlThemeChangedMi = GetMethodInfo<StyledElement>("OnControlThemeChanged");
        private static readonly MethodInfo? StyledElementOnTemplatedParentControlThemeChangedMi = GetMethodInfo<StyledElement>("OnTemplatedParentControlThemeChanged");


        public static Type? GetTargetType(this Selector selector)
        {
            if (SelectorTargetTypePi == null)
            {
                throw new NotSupportedException(
                    $"Can not find the 'TargetType' property in the type of '{nameof(Selector)}'.");
            }

            var targetType = SelectorTargetTypePi.GetValue(selector) as Type;
            return targetType;
        }

        public static void DetachStyles(this StyledElement element, IReadOnlyList<IStyle> styles)
        {
            if (DetachStylesMi == null)
            {
                throw new NotSupportedException(
                    $"Can not find the DetachStyles method in the type of '{nameof(StyledElement)}'.");
            }

            DetachStylesMi.Invoke(element, new object[] { styles });
        }

        public static void InvalidStyles(this StyledElement element)
        {
            if (StyledElementInvalidateStylesMi == null)
            {
                throw new NotSupportedException(
                    $"Can not find the InvalidateStyles method in the type of '{nameof(StyledElement)}'.");
            }

            StyledElementInvalidateStylesMi.Invoke(element, new object[] { true });
        }

        public static void OnControlThemeChanged(this StyledElement element)
        {
            if (StyledElementOnControlThemeChangedMi == null)
            {
                throw new NotSupportedException(
                    $"Can not find the OnControlThemeChanged method in the type of '{nameof(StyledElement)}'.");
            }

            StyledElementOnControlThemeChangedMi.Invoke(element, null);
        }

        public static void OnTemplatedParentControlThemeChanged(this StyledElement element)
        {
            if (StyledElementOnTemplatedParentControlThemeChangedMi == null)
            {
                throw new NotSupportedException(
                    $"Can not find the OnTemplatedParentControlThemeChanged method in the type of '{nameof(StyledElement)}'.");
            }

            StyledElementOnTemplatedParentControlThemeChangedMi.Invoke(element, null);
        }



        private static PropertyInfo? GetPropertyInfo<T>(string name) where T : class
        {
            return typeof(T).GetProperty(name,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private static MethodInfo? GetMethodInfo<T>(string name) where T : class
        {
            return typeof(T).GetMethod(name,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }
}