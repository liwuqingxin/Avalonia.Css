using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class CanvasLayout : IMagicLayout
{
    public IEnumerable<string> GetNames()
    {
        yield return "Canvas";
    }

    public Size MeasureOverride(MagicPanel panel, Size availableSize, IReadOnlyList<Control> children)
    {
        return new Size();
    }

    public Size ArrangeOverride(MagicPanel panel, Size finalSize, IReadOnlyList<Control> children)
    {
        foreach (var child in children)
        {
            ArrangeChild(child, finalSize);
        }

        return finalSize;
    }

    private static void ArrangeChild(Layoutable child, Size finalSize)
    {
        var x    = 0.0;
        var y    = 0.0;
        var left = Canvas.GetLeft(child);
        if (!double.IsNaN(left))
        {
            x = left;
        }
        else
        {
            var right = Canvas.GetRight(child);
            if (!double.IsNaN(right))
            {
                x = finalSize.Width - child.DesiredSize.Width - right;
            }
        }
        var top = Canvas.GetTop(child);
        if (!double.IsNaN(top))
        {
            y = top;
        }
        else
        {
            var bottom = Canvas.GetBottom(child);
            if (!double.IsNaN(bottom))
            {
                y = finalSize.Height - child.DesiredSize.Height - bottom;
            }
        }
        child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
    }
    
    public IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }

    public void ApplySetter(MagicPanel panel, string property, string value)
    {
        return;
    }
}