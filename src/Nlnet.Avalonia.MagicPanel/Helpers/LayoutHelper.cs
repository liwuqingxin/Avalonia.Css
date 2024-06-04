using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Layout;

namespace Nlnet.Avalonia;

internal static class LayoutHelper
{
    public static Point GetTopLeft(Layoutable child, Size finalSize)
    {
        var x = 0.0;
        var y = 0.0;
        
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

        return new Point(x, y);
    }

    public static void ApplyAlignmentToChild(Layoutable child, Alignment alignment, bool isHorizontal)
    {
        try
        {
            if (isHorizontal)
            {
                var diagnosis = child.GetDiagnostic(Layoutable.VerticalAlignmentProperty);
                if (diagnosis.Priority == BindingPriority.Unset)
                {
                    child.SetCurrentValue(Layoutable.VerticalAlignmentProperty, alignment.ToVertical());
                }
            }
            else
            {
                var diagnosis = child.GetDiagnostic(Layoutable.HorizontalAlignmentProperty);
                if (diagnosis.Priority == BindingPriority.Unset)
                {
                    child.SetCurrentValue(Layoutable.HorizontalAlignmentProperty, alignment.ToHorizontal());
                }
            }
        }
        catch
        {
            // ignore
        }
    }
}