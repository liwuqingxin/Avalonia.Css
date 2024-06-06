using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class StackLayout : LinearPanel, IMagicLayout
{
    public static StackLayout Default { get; } = new();

    private StackLayout()
    {
        
    }

    
    
    public IEnumerable<string> GetNames()
    {
        yield return "Stack";
        yield return "StackPanel";
    }

    public Size MeasureOverride(MagicPanel panel, Size availableSize, IReadOnlyList<Control> children)
    {
        var orientation  = GetOrientation(panel);
        var spacing      = GetSpacing(panel);
        var alignment    = GetItemsAlignment(panel);
        var isHorizontal = orientation == Orientation.Horizontal;
        
        var existedVisible     = false;
        var panelDesiredWidth  = 0d;
        var panelDesiredHeight = 0d;
        var index              = 0;
        
        // Measure all children with stackPanel's constraint.
        var constraintSize = isHorizontal
            ? availableSize.WithWidth(double.PositiveInfinity)
            : availableSize.WithHeight(double.PositiveInfinity);
        foreach (var child in children)
        {
            if (child.IsVisible)
            {
                existedVisible = true;
            }
            child.Measure(constraintSize);
        }

        // Constraint all.
        var constraintWidth  = 0d;
        var constraintHeight = 0d;
        if (isHorizontal)
        {
            constraintWidth  = double.PositiveInfinity;
            constraintHeight = availableSize.Height;
            if (double.IsInfinity(constraintHeight))
            {
                constraintHeight = children.Max(control => control.DesiredSize.Height);
            }
        }
        else
        {
            constraintHeight = double.PositiveInfinity;
            constraintWidth  = availableSize.Width;
            if (double.IsInfinity(constraintWidth))
            {
                constraintWidth = children.Max(control => control.DesiredSize.Width);
            }
        }
        constraintSize = new Size(constraintWidth, constraintHeight);

        for (var count = children.Count; index < count; ++index)
        {
            var child       = children[index];
            var isVisible   = child.IsVisible;
            var desiredSize = child.DesiredSize;
            
            // Location
            var isAlignItemsStretch = false;
            var childAlignment      = MagicPanel.GetAlignment(child); 
            if (isHorizontal)
            {
                var start = LocateStartWithAlignment(alignment, childAlignment, constraintSize.Height, desiredSize.Height, out isAlignItemsStretch);
                Canvas.SetLeft(child, panelDesiredWidth);
                Canvas.SetTop(child, start);
                
                panelDesiredWidth = panelDesiredWidth + (isVisible ? spacing : 0.0) + desiredSize.Width;
                panelDesiredHeight = Math.Max(panelDesiredHeight, desiredSize.Height);
            }
            else
            {
                var start = LocateStartWithAlignment(alignment, childAlignment, constraintSize.Width, desiredSize.Width, out isAlignItemsStretch);
                Canvas.SetLeft(child, start);
                Canvas.SetTop(child, panelDesiredHeight);
                
                panelDesiredWidth  = Math.Max(panelDesiredWidth, desiredSize.Width);
                panelDesiredHeight = panelDesiredHeight + (isVisible ? spacing : 0.0) + desiredSize.Height;
            }
            
            // Size
            var width  = child.DesiredSize.Width;
            var height = child.DesiredSize.Height;
            if (isAlignItemsStretch)
            {
                // TODO Test for availableSize.
                if (isHorizontal)
                {
                    height = constraintSize.Height;
                }
                else
                {
                    width = constraintSize.Width;
                }    
            }
            
            MagicPanel.SetArrangedWidth(child, width);
            MagicPanel.SetArrangedHeight(child, height);
        }

        var size = !isHorizontal
            ? new Size(panelDesiredWidth, panelDesiredHeight - (existedVisible ? spacing : 0.0))
            : new Size(panelDesiredWidth - (existedVisible ? spacing : 0.0), panelDesiredHeight);

        return size;
    }

    private static double LocateStartWithAlignment(
        Alignment alignment,
        Alignment? childAlignment,
        double constraint,
        double desired,
        out bool isAlignItemsStretch)
    {
        var align = childAlignment ?? alignment;
        var start = 0d;
        isAlignItemsStretch = false;
        
        switch (align)
        {
            case Alignment.Start:
                start = 0d;
                break;
            case Alignment.End:
                start = constraint - desired;
                break;
            case Alignment.Stretch:
                isAlignItemsStretch = true;
                break;
            case Alignment.Center:
                start = (constraint - desired) / 2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return start;
    }

    public Size ArrangeOverride(MagicPanel panel, Size finalSize, IReadOnlyList<Control> children)
    {        
        foreach (var child in children)
        {
            if (child.IsEffectivelyVisible == false)
            {
                continue;
            }
            
            var location = LayoutHelper.GetTopLeft(child, finalSize);
            var width    = MagicPanel.GetArrangedWidth(child);
            var height   = MagicPanel.GetArrangedHeight(child);
            
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
    
    public IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }

    public void ApplySetter(MagicPanel panel, string property, string value)
    {
        switch (property.ToLower())
        {
            case "orientation":
            case "direction":
            {
                ApplyOrientation(panel, value);
                break;
            }
            case "space":
            case "spacing":
            case "gap":
            {
                ApplySpacing(panel, value);
                break;
            }
            case "align":
            case "alignitems":
            case "alignment":
            case "itemsalignment":
            {
                ApplyItemsAlignment(panel, value);
                break;
            }
        }
    }
}