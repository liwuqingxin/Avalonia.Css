using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

internal struct FlexLine
{
    public List<Control> Line { get; }
    
    public double CrossSize { get; set; }

    public FlexLine()
    {
        Line      = new List<Control>();
        CrossSize = 0;
    }
}

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
        maca.SetMav(ref constraintSize, double.PositiveInfinity);
    }

    public static void ConstraintCrossAxisWithChildrenMaxDesiredIfNotConstraint(this ref Size constraintSize, IReadOnlyList<Control> children, IMaCa maca)
    {
        if (double.IsFinite(maca.CaV(constraintSize)))
        {
            return;
        }
        
        var value = children.Max(control => maca.CaV(control.DesiredSize));
        maca.SetCav(ref constraintSize, value);
    }

    public static double GetFlexBasis(this Control control, IMaCa maca)
    {
        var initialSize = MagicPanel.GetFlexBasis(control);
        if (double.IsNaN(initialSize))
        {
            initialSize = maca.MaV(control.DesiredSize);
        }

        return initialSize;
    }

    public static int GetJustifyContentSpacePieces(this JustifyContent justifyContent, int count)
    {
        return justifyContent switch
        {
            JustifyContent.Center => 2,
            JustifyContent.Start => 1,
            JustifyContent.End => 1,
            JustifyContent.SpaceEvenly => count + 1,
            JustifyContent.SpaceBetween => Math.Max(count - 1, 1),
            JustifyContent.SpaceAround => count * 2,
            _ => throw new ArgumentOutOfRangeException(nameof(justifyContent), justifyContent, null)
        };
    }

    public static double GetJustifyContentStart(this JustifyContent justifyContent, double spacePerPiece)
    {
        return justifyContent switch
        {
            JustifyContent.Center => spacePerPiece,
            JustifyContent.Start => 0,
            JustifyContent.End => spacePerPiece,
            JustifyContent.SpaceEvenly => spacePerPiece,
            JustifyContent.SpaceBetween => 0,
            JustifyContent.SpaceAround => spacePerPiece,
            _ => throw new ArgumentOutOfRangeException(nameof(justifyContent), justifyContent, null)
        };
    }
    
    public static double GetJustifyContentSpaceBetween(this JustifyContent justifyContent, double spacePerPiece)
    {
        return justifyContent switch
        {
            JustifyContent.Center => 0,
            JustifyContent.Start => 0,
            JustifyContent.End => 0,
            JustifyContent.SpaceEvenly => spacePerPiece,
            JustifyContent.SpaceBetween => spacePerPiece,
            JustifyContent.SpaceAround => spacePerPiece * 2,
            _ => throw new ArgumentOutOfRangeException(nameof(justifyContent), justifyContent, null)
        };
    }
    
    public static IList<double> ComputeSizes(this IReadOnlyList<Control> children, double spaced, IMaCa maca)
    {
        switch (spaced)
        {
            case 0:
            {
                return children.Select(c => c.GetFlexBasis(maca)).ToList();
            }
            case > 0:
            {
                var pieces = children.Sum(MagicPanel.GetFlexGrow);
                if (pieces == 0)
                {
                    return children.Select(c => c.GetFlexBasis(maca)).ToList();
                }

                var spacePerPiece = spaced / Math.Max(pieces, 1);
                return children.Select(c => c.GetFlexBasis(maca) + MagicPanel.GetFlexGrow(c) * spacePerPiece).ToList();
            }
            case < 0:
            {
                var pieces = children.Sum(MagicPanel.GetFlexShrink);
                if (pieces == 0)
                {
                    return children.Select(c => c.GetFlexBasis(maca)).ToList();
                }

                var spacePerPiece = spaced / Math.Max(pieces, 1);
                return children.Select(c => c.GetFlexBasis(maca) + MagicPanel.GetFlexShrink(c) * spacePerPiece).ToList();
            }
        }

        throw new NotImplementedException();
    }
    
    public static IEnumerable<FlexLine> WrapToLines(this IReadOnlyList<Control> children, Size availableSize, double gap, IMaCa maca)
    {
        var flexLine       = new FlexLine();
        var lineConstraint = maca.MaV(availableSize);
        var lineSize       = 0d;
        foreach (var child in children)
        {
            var desiredSize = child.GetFlexBasis(maca);
            lineSize += desiredSize;
            if (lineSize <= lineConstraint)
            {
                flexLine.Line.Add(child);
                lineSize           += gap;
                flexLine.CrossSize =  Math.Max(flexLine.CrossSize, maca.CaV(child.DesiredSize));
            }
            else
            {
                if (flexLine.Line.Count == 0)
                {
                    flexLine.Line.Add(child);
                    flexLine.CrossSize = Math.Max(flexLine.CrossSize, maca.CaV(child.DesiredSize));
                    yield return flexLine;
                    flexLine = new FlexLine();
                    lineSize = 0d;
                }
                else
                {
                    yield return flexLine;
                    flexLine = new FlexLine();
                    lineSize = desiredSize;
                    flexLine.Line.Add(child);
                    lineSize           += gap;    
                    flexLine.CrossSize =  Math.Max(flexLine.CrossSize, maca.CaV(child.DesiredSize));
                }
            }
        }

        yield return flexLine;
    }
    
    public static int GetAlignContentSpacePieces(this AlignContent alignContent, int count)
    {
        return alignContent switch
        {
            AlignContent.Stretch      => -1,
            AlignContent.Center       => 2,
            AlignContent.Start        => 1,
            AlignContent.End          => 1,
            AlignContent.SpaceEvenly  => count + 1,
            AlignContent.SpaceBetween => Math.Max(count - 1, 1),
            AlignContent.SpaceAround  => count * 2,
            _                         => throw new ArgumentOutOfRangeException(nameof(alignContent), alignContent, null)
        };
    }

    public static double GetAlignContentStart(this AlignContent alignContent, double spacePerPiece)
    {
        return alignContent switch
        {
            AlignContent.Stretch      => 0,
            AlignContent.Center       => spacePerPiece,
            AlignContent.Start        => 0,
            AlignContent.End          => spacePerPiece,
            AlignContent.SpaceEvenly  => spacePerPiece,
            AlignContent.SpaceBetween => 0,
            AlignContent.SpaceAround  => spacePerPiece,
            _                         => throw new ArgumentOutOfRangeException(nameof(alignContent), alignContent, null)
        };
    }
    
    public static double GetAlignContentSpaceBetween(this AlignContent alignContent, double spacePerPiece)
    {
        return alignContent switch
        {
            AlignContent.Stretch      => 0,
            AlignContent.Center       => 0,
            AlignContent.Start        => 0,
            AlignContent.End          => 0,
            AlignContent.SpaceEvenly  => spacePerPiece,
            AlignContent.SpaceBetween => spacePerPiece,
            AlignContent.SpaceAround  => spacePerPiece * 2,
            _                         => throw new ArgumentOutOfRangeException(nameof(alignContent), alignContent, null)
        };
    }
}