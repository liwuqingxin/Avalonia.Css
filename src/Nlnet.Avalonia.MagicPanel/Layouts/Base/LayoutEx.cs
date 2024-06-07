using System;
using Avalonia;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class LayoutEx : AvaloniaObject
{
    // ArrangedWidth
    public static double GetArrangedWidth(Layoutable host)
    {
        return host.GetValue(ArrangedWidthProperty);
    }
    internal static void SetArrangedWidth(Layoutable host, double value)
    {
        host.SetValue(ArrangedWidthProperty, value);
    }
    public static readonly AttachedProperty<double> ArrangedWidthProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, Layoutable, double>("ArrangedWidth", double.NaN);

    // ArrangedHeight
    public static double GetArrangedHeight(Layoutable host)
    {
        return host.GetValue(ArrangedHeightProperty);
    }
    internal static void SetArrangedHeight(Layoutable host, double value)
    {
        host.SetValue(ArrangedHeightProperty, value);
    }
    public static readonly AttachedProperty<double> ArrangedHeightProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, Layoutable, double>("ArrangedHeight", double.NaN);
    
    // Spacing
    public static double GetSpacing(MagicPanel host)
    {
        return host.GetValue(SpacingProperty);
    }
    public static void SetSpacing(MagicPanel host, double value)
    {
        host.SetValue(SpacingProperty, value);
    }
    public static readonly AttachedProperty<double> SpacingProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, double>("Spacing");
    
    // ItemsAlignment
    public static Alignment GetItemsAlignment(MagicPanel host)
    {
        return host.GetValue(ItemsAlignmentProperty);
    }
    public static void SetItemsAlignment(MagicPanel host, Alignment value)
    {
        host.SetValue(ItemsAlignmentProperty, value);
    }
    public static readonly AttachedProperty<Alignment> ItemsAlignmentProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, Alignment>("ItemsAlignment");
    
    // Orientation
    public static Orientation GetOrientation(MagicPanel host)
    {
        return host.GetValue(OrientationProperty);
    }
    public static void SetOrientation(MagicPanel host, Orientation value)
    {
        host.SetValue(OrientationProperty, value);
    }
    public static readonly AttachedProperty<Orientation> OrientationProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, Orientation>("Orientation", Orientation.Vertical);

    // Reverse
    public static bool GetReverse(MagicPanel host)
    {
        return host.GetValue(ReverseProperty);
    }
    public static void SetReverse(MagicPanel host, bool value)
    {
        host.SetValue(ReverseProperty, value);
    }
    public static readonly AttachedProperty<bool> ReverseProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, bool>("Reverse");
}

internal static class LayoutExtensions
{
    public static void ApplySpacing(this MagicPanel panel, string value)
    {
        if (double.TryParse(value, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(LayoutEx.SpacingProperty, v);
    }

    public static void ApplyItemsAlignment(this MagicPanel panel, string value)
    {
        if (Enum.TryParse<Alignment>(value, true, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(LayoutEx.ItemsAlignmentProperty, v);
    }
    
    public static void ApplyOrientation(this MagicPanel panel, string value)
    {
        switch (value)
        {
            case "v" or "V":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Vertical);
                break;
            case "h" or "H":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Horizontal);
                break;
            default:
            {
                if (Enum.TryParse<Orientation>(value, true, out var v) == false)
                {
                    return;
                }

                panel.SetCurrentValue(LayoutEx.OrientationProperty, v);
                break;
            }
        }
    }

    public static void ApplyReverse(this MagicPanel panel, string value)
    {
        var reverse = string.IsNullOrEmpty(value) || (bool.TryParse(value, out var b) && b);
        panel.SetCurrentValue(LayoutEx.ReverseProperty, reverse);
    }
}