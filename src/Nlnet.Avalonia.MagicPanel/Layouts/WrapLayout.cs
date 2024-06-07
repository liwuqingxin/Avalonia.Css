using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Input;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class WrapLayout : MagicLayout
{
    public static WrapLayout Default { get; } = new();

    private WrapLayout()
    {
        
    }
    
    
    
    public override IEnumerable<string> GetNames()
    {
        yield return "Wrap";
        yield return "WrapPanel";
    }

    public override Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize)
    {
        var availableWidth = availableSize.Width;
        if (double.IsNaN(availableWidth))
        {
            availableWidth = double.MaxValue;
        }
        
        var width      = 0d;
        var height     = 0d;
        var lineWidth  = 0d;
        var lineHeight = 0d;
        var lineCount  = 0;
        
        foreach (var child in children)
        {
            child.Measure(availableSize);
            
            if (lineWidth + child.DesiredSize.Width > availableWidth)
            {
                if (lineCount == 0)
                {
                    Canvas.SetTop(child, height);
                    Canvas.SetLeft(child, 0);
                    
                    width  =  Math.Max(width, child.DesiredSize.Width);
                    height += child.DesiredSize.Height;
                }
                else
                {
                    width  =  Math.Max(width, lineWidth);
                    height += lineHeight;
                    
                    Canvas.SetTop(child, height);
                    Canvas.SetLeft(child, 0);
                }
                
                lineWidth  = child.DesiredSize.Width;
                lineHeight = child.DesiredSize.Height;
                lineCount  = 1;
            }
            else
            {
                Canvas.SetTop(child, height);
                Canvas.SetLeft(child, lineWidth);
                
                lineCount++;
                lineWidth  += child.DesiredSize.Width;
                lineHeight =  Math.Max(lineHeight, child.DesiredSize.Height);
            }
        }

        return new Size(width, height);
    }

    public override Size ArrangeOverride(MagicPanel panel, IReadOnlyList<Control> children, Size finalSize)
    {
        var isHorizontal = LayoutEx.GetOrientation(panel) == Orientation.Horizontal;
        
        foreach (var child in children)
        {
            ArrangeChild(panel, child, finalSize, isHorizontal);
        }

        return finalSize;
    }

    public override IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }

    private static void ArrangeChild(MagicPanel panel, Layoutable child, Size finalSize, bool isHorizontal)
    {
        var location = child.GetTopLeft(finalSize);
        
        var width  = child.DesiredSize.Width;
        var height = child.DesiredSize.Height;
        if (isHorizontal)
        {
            height = finalSize.Height;
        }
        else
        {
            width = finalSize.Width;
        }

        // ApplyParentPanel's AlignItems.
        var alignment = LayoutEx.GetAlignItems(panel);
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
        
        child.Arrange(new Rect(location, new Size(width, height)));
    }
}