using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public abstract class ReactiveLayout : AvaloniaObject, IMagicLayout
{
    public static double GetSpacing(MagicPanel host)
    {
        return host.GetValue(SpacingProperty);
    }
    public static void SetSpacing(MagicPanel host, double value)
    {
        host.SetValue(SpacingProperty, value);
    }
    public static readonly AttachedProperty<double> SpacingProperty = AvaloniaProperty
        .RegisterAttached<ReactiveLayout, MagicPanel, double>("Spacing");
    
    public static Alignment GetItemsAlignment(MagicPanel host)
    {
        return host.GetValue(ItemsAlignmentProperty);
    }
    public static void SetItemsAlignment(MagicPanel host, Alignment value)
    {
        host.SetValue(ItemsAlignmentProperty, value);
    }
    public static readonly AttachedProperty<Alignment> ItemsAlignmentProperty = AvaloniaProperty
        .RegisterAttached<ReactiveLayout, MagicPanel, Alignment>("ItemsAlignment");
    
    
    
    #region IMagicLayout

    public abstract IEnumerable<string> GetNames();

    public abstract Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize);

    public virtual Size ArrangeOverride(MagicPanel panel, IReadOnlyList<Control> children, Size finalSize)
    {
        foreach (var child in children)
        {
            if (child.IsEffectivelyVisible == false)
            {
                continue;
            }
            
            var location = child.GetTopLeft(finalSize);
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

    public abstract IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap);

    public virtual void ApplySetter(MagicPanel panel, string property, string value)
    {
        switch (property)
        {
            case "space":
            case "spacing":
            case "gap":
            {
                ApplySpacing(panel, value);
                break;
            }
            case "align":
            case "alignment":
            case "alignchild":
            case "alignitems":
            case "itemsalignment":
            {
                ApplyItemsAlignment(panel, value);
                break;
            }
        }
    }

    private static void ApplySpacing(MagicPanel panel, string value)
    {
        if (double.TryParse(value, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(SpacingProperty, v);
    }

    private static void ApplyItemsAlignment(MagicPanel panel, string value)
    {
        if (Enum.TryParse<Alignment>(value, true, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(ItemsAlignmentProperty, v);
    }

    #endregion
}