using System;
using Avalonia;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class ReactivePanel : AvaloniaObject
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
        .RegisterAttached<ReactivePanel, MagicPanel, double>("Spacing");
    
    public static Alignment GetItemsAlignment(MagicPanel host)
    {
        return host.GetValue(ItemsAlignmentProperty);
    }
    public static void SetItemsAlignment(MagicPanel host, Alignment value)
    {
        host.SetValue(ItemsAlignmentProperty, value);
    }
    public static readonly AttachedProperty<Alignment> ItemsAlignmentProperty = AvaloniaProperty
        .RegisterAttached<ReactivePanel, MagicPanel, Alignment>("ItemsAlignment");
    
    
    
    protected static void ApplySpacing(MagicPanel panel, string value)
    {
        if (double.TryParse(value, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(SpacingProperty, v);
    }
    
    protected static void ApplyItemsAlignment(MagicPanel panel, string value)
    {
        if (Enum.TryParse<Alignment>(value, true, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(ItemsAlignmentProperty, v);
    }
}