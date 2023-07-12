using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
        /// Get the list of <see cref="ICssStyle"/> instance that would be adding to the host.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        internal static IList<ICssStyle>? GetAddingStyle(StyledElement host)
        {
            return host.GetValue(AddingStyleProperty);
        }
        /// <summary>
        /// Set the list of <see cref="ICssStyle"/> instance that would be adding to the host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="value"></param>
        internal static void SetAddingStyle(StyledElement host, IList<ICssStyle>? value)
        {
            host.SetValue(AddingStyleProperty, value);
        }
        internal static readonly AttachedProperty<IList<ICssStyle>?> AddingStyleProperty = AvaloniaProperty
            .RegisterAttached<ExStyler, StyledElement, IList<ICssStyle>?>("AddingStyle");



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
                element.ClearValue(AddingStyleProperty);
                if (args.NewValue is IList<ICssStyle> cssStyleList)
                {
                    foreach (var cssStyle in cssStyleList)
                    {
                        // 'element.Styles.Add(s)' will cause AddingStyleProperty being changed, which should be passed.
                        if (element.Styles.OfType<ChildStyle>().Any(child => child.CssStyle == cssStyle))
                        {
                            continue;
                        }
                    
                        var s = cssStyle.ToAvaloniaStyle();
                        element.Styles.Add(s);
                        cssStyle.AddDisposable(Disposable.Create(() =>
                        {
                            element.Styles.Remove(s);
                        }));   
                    }
                }
            });
        }
    }
}
