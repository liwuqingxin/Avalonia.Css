using System;
using System.Collections.Generic;
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
        var isHorizontal = orientation == Orientation.Horizontal;
        
        var desiredWidth   = 0d;
        var desiredHeight  = 0d;
        var existedVisible = false;
        var index          = 0;

        var availableSize1 = isHorizontal
            ? availableSize.WithWidth(double.PositiveInfinity)
            : availableSize.WithHeight(double.PositiveInfinity);

        for (var count = children.Count; index < count; ++index)
        {
            var child     = children[index];
            var isVisible = child.IsVisible;
            if (isVisible)
            {
                existedVisible = true;
            }
            
            // LayoutHelper.ApplyAlignmentToChild(control, GetItemsAlignment(panel), isHorizontal);

            child.Measure(availableSize1);
            
            var desiredSize = child.DesiredSize;
            
            // Location
            // TODO Consider the alignment of the child and the panel.
            if (isHorizontal)
            {
                Canvas.SetLeft(child, desiredWidth);
                Canvas.SetTop(child, 0);
                
                desiredWidth = desiredWidth + (isVisible ? spacing : 0.0) + desiredSize.Width;
                desiredHeight = Math.Max(desiredHeight, desiredSize.Height);
            }
            else
            {
                Canvas.SetLeft(child, 0);
                Canvas.SetTop(child, desiredHeight);
                
                desiredWidth  = Math.Max(desiredWidth, desiredSize.Width);
                desiredHeight = desiredHeight + (isVisible ? spacing : 0.0) + desiredSize.Height;
            }
            
            // Size
            // TODO Test for availableSize.
            var width  = child.DesiredSize.Width;
            var height = child.DesiredSize.Height;
            if (isHorizontal)
            {
                height = availableSize.Height;
            }
            else
            {
                width = availableSize.Width;
            }
            
            MagicPanel.SetArrangedWidth(child, width);
            MagicPanel.SetArrangedHeight(child, height);
        }

        var size = !isHorizontal
            ? new Size(desiredWidth, desiredHeight - (existedVisible ? spacing : 0.0))
            : new Size(desiredWidth - (existedVisible ? spacing : 0.0), desiredHeight);

        return size;
    }

    public Size ArrangeOverride(MagicPanel panel, Size finalSize, IReadOnlyList<Control> children)
    {        
        foreach (var child in children)
        {
            var location = LayoutHelper.GetTopLeft(child, finalSize);
            var width    = MagicPanel.GetArrangedWidth(child);
            var height   = MagicPanel.GetArrangedHeight(child);

            if (double.IsNaN(width))
            {
                width = finalSize.Width;
            }
            
            if (double.IsNaN(height))
            {
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