using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Controls;

public class DefaultLayoutDefinition : ILayoutDefinition
{
    public static DefaultLayoutDefinition Default { get; } = new();

    private DefaultLayoutDefinition()
    {
        
    }
    
    public string GetName()
    {
        return "Default";
    }

    public Size MeasureOverride(Size availableSize, IReadOnlyList<Control> children)
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

    public Size ArrangeOverride(Size finalSize, IReadOnlyList<Control> children)
    {
        return new Size();
    }
}