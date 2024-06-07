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

    public static void JustMeasure(this IReadOnlyList<Control> children, Size constraint, out bool existedVisible)
    {
        existedVisible = false;
        foreach (var child in children.Where(child => child.IsVisible))
        {
            existedVisible = true;
            child.Measure(constraint);
        }
    }

    public static void LiberateExtendDirection(this ref Size constraintSize, bool isHorizontal)
    {
        constraintSize = isHorizontal
            ? constraintSize.WithWidth(double.PositiveInfinity)
            : constraintSize.WithHeight(double.PositiveInfinity);
    }

    public static void ConstraintNoExtendDirectionWithChildrenMaxDesiredIfNotConstraint(this ref Size constraintSize, IReadOnlyList<Control> children, bool isHorizontal)
    {
        var constraintWidth  = constraintSize.Width;
        var constraintHeight = constraintSize.Height;
        
        if (isHorizontal)
        {
            if (double.IsInfinity(constraintHeight))
            {
                constraintSize = new Size(constraintWidth, children.Max(control => control.DesiredSize.Height));
            }
        }
        else
        {
            if (double.IsInfinity(constraintWidth))
            {
                constraintSize  = new Size(children.Max(control => control.DesiredSize.Width), constraintHeight);
            }
        }
        
    }
}