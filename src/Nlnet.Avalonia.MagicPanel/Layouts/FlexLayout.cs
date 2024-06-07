using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class FlexLayout : MagicLayout
{
    public static FlexLayout Default { get; } = new();
    
    private FlexLayout()
    {
        
    }
    
    public override IEnumerable<string> GetNames()
    {
        yield return "Flex";
        yield return "FlexPanel";
    }

    public override Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize)
    {
        var orientation  = LayoutEx.GetOrientation(panel);
        var isHorizontal = orientation == Orientation.Horizontal;
        
        // If the extend direction has no restriction, just regard as stack layout.
        if (isHorizontal)
        {
            if (double.IsInfinity(availableSize.Width))
            {
                return StackLayout.Default.MeasureOverride(panel, children, availableSize);
            }
        }
        else
        {
            if (double.IsInfinity(availableSize.Height))
            {
                return StackLayout.Default.MeasureOverride(panel, children, availableSize);
            }
        }
        
        var spacing   = LayoutEx.GetSpacing(panel);
        var alignment = LayoutEx.GetItemsAlignment(panel);

        var panelDesiredWidth  = 0d;
        var panelDesiredHeight = 0d;
        var index              = 0;

        // Measure all children.
        var constraintSize = availableSize;
        children.JustMeasure(constraintSize, out var existedVisible);

        // Constraint all.
        constraintSize.ConstraintNoExtendDirectionWithChildrenMaxDesiredIfNotConstraint(children, isHorizontal);

        for (var count = children.Count; index < count; ++index)
        {
            var child       = children[index];
            var desiredSize = child.DesiredSize;
            
            // Location
            var isStretch      = false;
            var childAlignment = MagicPanel.GetAlignment(child); 
            if (isHorizontal)
            {
                var start = LayoutHelper.LocateStartWithAlignment(alignment, childAlignment, constraintSize.Height, desiredSize.Height, out isStretch);
                Canvas.SetLeft(child, panelDesiredWidth);
                Canvas.SetTop(child, start);
                
                panelDesiredWidth  = panelDesiredWidth + spacing + desiredSize.Width;
                panelDesiredHeight = Math.Max(panelDesiredHeight, desiredSize.Height);
            }
            else
            {
                var start = LayoutHelper.LocateStartWithAlignment(alignment, childAlignment, constraintSize.Width, desiredSize.Width, out isStretch);
                Canvas.SetLeft(child, start);
                Canvas.SetTop(child, panelDesiredHeight);
                
                panelDesiredWidth  = Math.Max(panelDesiredWidth, desiredSize.Width);
                panelDesiredHeight = panelDesiredHeight + spacing + desiredSize.Height;
            }
            
            // Size
            var width  = child.DesiredSize.Width;
            var height = child.DesiredSize.Height;
            if (isStretch)
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
            
            LayoutEx.SetArrangedWidth(child, width);
            LayoutEx.SetArrangedHeight(child, height);
        }

        var size = isHorizontal
            ? new Size(panelDesiredWidth - (existedVisible ? spacing : 0.0), panelDesiredHeight)
            : new Size(panelDesiredWidth, panelDesiredHeight - (existedVisible ? spacing : 0.0));

        return size;
    }

    public override IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap)
    {
        return null;
    }



    private static List<double> CalculatePosition(IList<Control> children, Size constraintSize, bool isHorizontal)
    {
        var points = new List<double>();
        if (isHorizontal)
        {
            var totalDesired = children.Sum(c => c.DesiredSize.Width);
            if (totalDesired >= constraintSize.Width)
            {
                var cursor = 0d;
                foreach (var child in children)
                {
                    points.Add(cursor);
                    cursor += child.DesiredSize.Width / totalDesired * constraintSize.Width;
                }
            }
            else
            {
                
            }
        }
        else
        {
            
        }

        return points;
    }
}