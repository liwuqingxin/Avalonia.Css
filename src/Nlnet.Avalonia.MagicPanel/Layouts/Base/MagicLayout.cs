using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Nlnet.Avalonia.Controls;
// ReSharper disable StringLiteralTypo

namespace Nlnet.Avalonia;

public abstract class MagicLayout : AvaloniaObject, IMagicLayout
{
    #region IMagicLayout

    public abstract IEnumerable<string> GetNames();

    public abstract Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize);

    public virtual Size ArrangeOverride(MagicPanel panel, IReadOnlyList<Control> children, Size finalSize)
    {
        foreach (var child in children)
        {
            if (child.IsEffectivelyVisible == false)
            {
                continue;
            }
            
            var location = child.GetTopLeft(finalSize);
            var width    = LayoutEx.GetArrangedWidth(child);
            var height   = LayoutEx.GetArrangedHeight(child);
            
            if (double.IsFinite(location.X) == false)
            {
                Trace.WriteLine("MagicPanel: location of X is not finite.");
                location = location.WithX(0);
            }
            if (double.IsFinite(location.Y) == false)
            {
                Trace.WriteLine("MagicPanel: location of Y is not finite.");
                location = location.WithY(0);
            }
            if (double.IsFinite(width) == false)
            {
                Trace.WriteLine("MagicPanel: width is not finite.");
                width = finalSize.Width;
            }
            if (double.IsFinite(height) == false)
            {
                Trace.WriteLine("MagicPanel: height is not finite.");
                height = finalSize.Height;
            }
        
            child.Arrange(new Rect(location, new Size(width, height)));
        }

        return finalSize;
    }

    public abstract IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap);

    public virtual void ApplySetter(MagicPanel panel, string property, string value)
    {
        switch (property)
        {
            case "space":
            case "spacing":
            case "gap":
            {
                panel.ApplySpacing(value);
                break;
            }
            case "align":
            case "alignment":
            case "alignchild":
            case "alignitems":
            case "align-child":
            case "align-items":
            {
                panel.ApplyItemsAlignment(value);
                break;
            }
            case "orientation":
            case "direction":
            case "flex-direction":
            {
                panel.ApplyOrientation(value);
                break;
            }
            case "r":
            case "reverse":
            {
                panel.ApplyReverse(value);
                break;
            }
        }
    }

    #endregion
}