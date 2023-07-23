using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Some helper methods or properties for <see cref="IStyle"/>.
    /// </summary>
    internal class ExStyler : AvaloniaObject
    {
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

                    StylerHelper.ReapplyStyling(element, false, true, true);
                }
            });
        }
    }
}
