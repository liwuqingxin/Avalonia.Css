using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class FlexLayout : StackLayout
{
    public new static FlexLayout Default { get; } = new();
    
    private FlexLayout()
    {
        
    }
    
    public override IEnumerable<string> GetNames()
    {
        yield return "Flex";
        yield return "FlexPanel";
    }

    public override Size MeasureOverride(MagicPanel panel, Size availableSize, IReadOnlyList<Control> children)
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
            ? availableSize.WithWidth(availableSize.Width / children.Count)
            : availableSize.WithHeight(availableSize.Height / children.Count);
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
}