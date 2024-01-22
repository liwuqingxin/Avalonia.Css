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

    
    
    public IEnumerable<string> GetNames()
    {
        yield return "Stack";
        yield return "StackPanel";
    }

    public Size MeasureOverride(MagicPanel panel, Size availableSize, IReadOnlyList<Control> children)
    {
        var orientation = GetOrientation(panel);
        var spacing     = GetSpacing(panel);
        
        var width          = 0d;
        var height         = 0d;
        var isHorizontal   = orientation == Orientation.Horizontal;
        var existedVisible = false;
        var index          = 0;

        var availableSize1 = !isHorizontal
            ? availableSize.WithHeight(double.PositiveInfinity)
            : availableSize.WithWidth(double.PositiveInfinity);

        for (var count = children.Count; index < count; ++index)
        {
            var control   = children[index];
            var isVisible = control.IsVisible;
            if (isVisible && !existedVisible)
            {
                existedVisible = true;
            }

            control.Measure(availableSize1);
            
            var desiredSize = control.DesiredSize;
            if (isHorizontal)
            {
                Canvas.SetLeft(control, width);
                Canvas.SetTop(control, 0);
                
                width = width + (isVisible ? spacing : 0.0) + desiredSize.Width;
                height = Math.Max(height, desiredSize.Height);
            }
            else
            {
                Canvas.SetLeft(control, 0);
                Canvas.SetTop(control, height);
                
                width  = Math.Max(width, desiredSize.Width);
                height = height + (isVisible ? spacing : 0.0) + desiredSize.Height;
            }
        }

        var size = !isHorizontal
            ? new Size(width, height - (existedVisible ? spacing : 0.0))
            : new Size(width - (existedVisible ? spacing : 0.0), height);

        return size;
    }

    public Size ArrangeOverride(MagicPanel panel, Size finalSize, IReadOnlyList<Control> children)
    {
        var isHorizontal = GetOrientation(panel) == Orientation.Horizontal;
        
        foreach (var child in children)
        {
            ArrangeChild(panel, child, finalSize, isHorizontal);
        }

        return finalSize;
    }

    private static void ArrangeChild(MagicPanel panel, Layoutable child, Size finalSize, bool isHorizontal)
    {
        var location = InternalLayoutHelper.GetLocation(child, finalSize);
        
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
        
        InternalLayoutHelper.ApplyAlignment(child, GetItemsAlignment(panel), isHorizontal);
        
        child.Arrange(new Rect(location, new Size(width, height)));
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