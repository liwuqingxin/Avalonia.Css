using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class StackLayout : LinearLayout
{
    public static StackLayout Default { get; } = new();

    private StackLayout()
    {
        
    }
    
    public override IEnumerable<string> GetNames()
    {
        yield return "Stack";
        yield return "StackPanel";
    }

    public override Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize)
    {
        var orientation  = GetOrientation(panel);
        var isHorizontal = orientation == Orientation.Horizontal;
        var spacing      = GetSpacing(panel);
        var alignment    = GetItemsAlignment(panel);

        var panelDesiredWidth  = 0d;
        var panelDesiredHeight = 0d;
        var index              = 0;
        
        // Measure all children.
        var constraintSize = availableSize;
        constraintSize.LiberateExtendDirection(isHorizontal);
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
            
            MagicPanel.SetArrangedWidth(child, width);
            MagicPanel.SetArrangedHeight(child, height);
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
}