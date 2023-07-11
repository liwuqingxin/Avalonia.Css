using System;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using DynamicData;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Some helper methods or properties for <see cref="IStyle"/>.
    /// </summary>
    internal class ExStyler : AvaloniaObject
    {
        /// <summary>
        /// Meaningless object. You should not get value from <see cref="IncludeProperty"/> normally.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Uri GetInclude(StyledElement host)
        {
            return host.GetValue(IncludeProperty);
        }
        /// <summary>
        /// Add a style resource to the <see cref="host"/>'s <see cref="StyledElement.Styles"/> if it is not existed.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="value"></param>
        public static void SetInclude(StyledElement host, Uri value)
        {
            host.SetValue(IncludeProperty, value);
        }
        public static readonly AttachedProperty<Uri> IncludeProperty = AvaloniaProperty
            .RegisterAttached<Styler, StyledElement, Uri>("Include");

        /// <summary>
        /// Get <see cref="Uri"/> of <see cref="StyleInclude"/> to remove.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Uri? GetRemoveInclude(StyledElement host)
        {
            return host.GetValue(RemoveIncludeProperty);
        }
        /// <summary>
        /// Set <see cref="Uri"/> of <see cref="StyleInclude"/> to remove.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="value"></param>
        public static void SetRemoveInclude(StyledElement host, Uri? value)
        {
            host.SetValue(RemoveIncludeProperty, value);
        }
        public static readonly AttachedProperty<Uri?> RemoveIncludeProperty = AvaloniaProperty
            .RegisterAttached<Styler, StyledElement, Uri?>("RemoveInclude");

        /// <summary>
        /// Get the <see cref="ICssStyle"/> instance that would be adding to the host.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        internal static ICssStyle GetAddingStyle(Visual host)
        {
            return host.GetValue(AddingStyleProperty);
        }
        /// <summary>
        /// Set the <see cref="ICssStyle"/> instance that would be adding to the host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="value"></param>
        internal static void SetAddingStyle(Visual host, ICssStyle value)
        {
            host.SetValue(AddingStyleProperty, value);
        }
        internal static readonly AttachedProperty<ICssStyle> AddingStyleProperty = AvaloniaProperty
            .RegisterAttached<ExStyler, Visual, ICssStyle>("AddingStyle");



        static ExStyler()
        {
            IncludeProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
            {
                if (args.NewValue is Uri uri && element.Styles.FirstOrDefault(s => s is StyleInclude include && include.Source == uri) == null)
                {
                    element.Styles.Add(new StyleInclude(uri)
                    {
                        Source = uri
                    });
                }
            });

            RemoveIncludeProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
            {
                if (args.NewValue is not Uri uri)
                {
                    return;
                }

                var include = element.Styles.OfType<StyleInclude>().FirstOrDefault(include => include.Source == uri);
                if (include != null)
                {
                    element.Styles.Remove(include);
                }

                // Maybe fail.
                element.SetValue(RemoveIncludeProperty, null, BindingPriority.Style);
            });

            AddingStyleProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
            {
                这里循环报错。考虑CssBuilder中记录所有添加动作，并添加对应的dispoasable；重新加载时直接dispose；
                if (args.NewValue is ICssStyle cssStyle)
                {
                    var exist = element.Styles.OfType<ChildStyle>().FirstOrDefault(s => s.CssStyle == cssStyle);
                    if (exist != null)
                    {
                        //return;
                        element.Styles.Remove(exist);
                    }

                    element.Styles.Add(cssStyle.ToAvaloniaStyle());
                }
            });
        }
    }
}
