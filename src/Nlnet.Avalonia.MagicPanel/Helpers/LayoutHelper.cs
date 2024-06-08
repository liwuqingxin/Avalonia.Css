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
        var left = LayoutEx.GetArrangedLeft(child);
        var top  = LayoutEx.GetArrangedTop(child);
        
        return new Point(left, top);
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
        LayoutEx.SetArrangedLeft(child, maca.TileXOrAlign(width, alignPoint));
        LayoutEx.SetArrangedTop(child, maca.TileYOrAlign(height, alignPoint));
    }
}