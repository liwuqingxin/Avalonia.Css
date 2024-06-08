using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Nlnet.Avalonia;

internal static class LayoutHelper
{
    public static double LocateStartWithAlignment(
        Alignment  alignment,
        Alignment? childAlignment,
        double     constraint,
        double     desired,
        out bool   isStretch)
    {
        var align = childAlignment ?? alignment;
        var start = 0d;
        isStretch = false;
        
        switch (align)
        {
            case Alignment.Start:
                start = 0d;
                break;
            case Alignment.End:
                start = constraint - desired;
                break;
            case Alignment.Stretch:
                start     = 0d;
                isStretch = true;
                break;
            case Alignment.Center:
                start = (constraint - desired) / 2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return start;
    }
    
    public static Point GetTopLeft(this Layoutable child, Size finalSize)
    {
        // TODO Use custom Top/Left instead of Canvas.Top and Canvas.Left. 
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

    public static void JustMeasure(this IReadOnlyList<Control> children, Size constraint)
    {
        foreach (var child in children)
        {
            child.Measure(constraint);
        }
    }

    public static void LiberateMainAxis(this ref Size constraintSize, IMaCa maca)
    {
        maca.WithMav(ref constraintSize, double.PositiveInfinity);
    }

    public static void ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(this ref Size constraintSize, IReadOnlyList<Control> children, IMaCa maca)
    {
        if (double.IsFinite(maca.CaV(constraintSize)))
        {
            return;
        }
        
        var value = children.Max(control => maca.CaV(control.DesiredSize));
        maca.WithCav(ref constraintSize, value);
    }

    public static void TileAndAlign(this Control child, double width, double height, double alignPoint, IMaCa maca)
    {
        Canvas.SetLeft(child, maca.TileXOrAlign(width, alignPoint));
        Canvas.SetTop(child, maca.TileYOrAlign(height, alignPoint));
    }
}