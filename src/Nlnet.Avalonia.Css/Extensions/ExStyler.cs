using System.Collections.Generic;
using System.Linq;
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
        /// Get the list of <see cref="IAcssStyle"/> instance that would be adding to the host.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        internal static IList<IAcssStyle>? GetAddingStyle(StyledElement host)
        {
            return host.GetValue(AddingStyleProperty);
        }
        /// <summary>
        /// Set the list of <see cref="IAcssStyle"/> instance that would be adding to the host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="value"></param>
        internal static void SetAddingStyle(StyledElement host, IList<IAcssStyle>? value)
        {
            host.SetValue(AddingStyleProperty, value);
        }
        internal static readonly AttachedProperty<IList<IAcssStyle>?> AddingStyleProperty = AvaloniaProperty
            .RegisterAttached<ExStyler, StyledElement, IList<IAcssStyle>?>("AddingStyle");



        static ExStyler()
        {
            AddingStyleProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
            {
                element.ClearValue(AddingStyleProperty);

                if (args.NewValue is IList<IAcssStyle> acssStyleList)
                {
                    foreach (var acssStyle in acssStyleList)
                    {
                        // 'element.Styles.Add(s)' will cause AddingStyleProperty being changed, which should be passed.
                        if (element.Styles.OfType<ChildStyle>().Any(child => child.AcssStyle == acssStyle))
                        {
                            continue;
                        }
                    
                        var s = acssStyle.ToAvaloniaStyle();
                        element.Styles.Add(s);
                        acssStyle.AddDisposable(Disposable.Create(() =>
                        {
                            element.Styles.Remove(s);
                        }));   
                    }

                    var targetTypes = acssStyleList.Select(s => s.GetTargetType()).ToList();
                    element.ReapplyStyling(false, false, false, true, targetTypes!);
                }
            });
        }
    }
}
