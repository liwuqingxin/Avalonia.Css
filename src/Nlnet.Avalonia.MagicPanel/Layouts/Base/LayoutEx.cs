using System;
using System.Diagnostics;
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
        .RegisterAttached<LayoutEx, MagicPanel, double>("Spacing", 0d);
    
    // AlignItems
    public static Alignment GetAlignItems(MagicPanel host)
    {
        return host.GetValue(AlignItemsProperty);
    }
    public static void SetAlignItems(MagicPanel host, Alignment value)
    {
        host.SetValue(AlignItemsProperty, value);
    }
    public static readonly AttachedProperty<Alignment> AlignItemsProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, Alignment>("AlignItems", Alignment.Stretch);

    // JustifyContent
    public static JustifyContent GetJustifyContent(MagicPanel host)
    {
        return host.GetValue(JustifyContentProperty);
    }
    public static void SetJustifyContent(MagicPanel host, JustifyContent value)
    {
        host.SetValue(JustifyContentProperty, value);
    }
    public static readonly AttachedProperty<JustifyContent> JustifyContentProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, JustifyContent>("JustifyContent", JustifyContent.Center);
    
    // AlignContent
    public static AlignContent GetAlignContent(MagicPanel host)
    {
        return host.GetValue(AlignContentProperty);
    }
    public static void SetAlignContent(MagicPanel host, AlignContent value)
    {
        host.SetValue(AlignContentProperty, value);
    }
    public static readonly AttachedProperty<AlignContent> AlignContentProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, AlignContent>("AlignContent", AlignContent.Stretch);
    
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
        .RegisterAttached<LayoutEx, MagicPanel, bool>("Reverse", false);

    // FlexWrap
    public static FlexWrap GetFlexWrap(MagicPanel host)
    {
        return host.GetValue(FlexWrapProperty);
    }
    public static void SetFlexWrap(MagicPanel host, FlexWrap value)
    {
        host.SetValue(FlexWrapProperty, value);
    }
    public static readonly AttachedProperty<FlexWrap> FlexWrapProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, MagicPanel, FlexWrap>("FlexWrap", FlexWrap.NoWrap);
}

internal static class LayoutExtensions
{
    private static bool TryApply<TEnum>(this MagicPanel panel, AvaloniaProperty property, string value) where TEnum : struct
    {
        if (Enum.TryParse<TEnum>(value, true, out var v) == false)
        {
            return false;
        }

        panel.SetCurrentValue(property, v);
        return true;
    }
    
    public static void ApplySpacing(this MagicPanel panel, string value)
    {
        if (double.TryParse(value, out var v) == false)
        {
            return;
        }

        panel.SetCurrentValue(LayoutEx.SpacingProperty, v);
    }

    public static void ApplyAlignItems(this MagicPanel panel, string value)
    {
        // align-items: flex-start | flex-end | center | baseline | stretch;
        switch (value)
        {
            case "flex-start":
                panel.SetCurrentValue(LayoutEx.AlignItemsProperty, Alignment.Start);
                break;
            case "flex-end":
                panel.SetCurrentValue(LayoutEx.AlignItemsProperty, Alignment.End);
                break;
            case "baseline":
                Trace.WriteLine("The baseline for align-items is not supported.");
                break;
            default:
                panel.TryApply<Alignment>(LayoutEx.AlignItemsProperty, value);
                break;
        }
    }

    public static void ApplyJustifyContent(this MagicPanel panel, string value)
    {
        // justify-content: flex-start | flex-end | center | space-between | space-around;
        switch (value)
        {
            case "flex-start":
                panel.SetCurrentValue(LayoutEx.JustifyContentProperty, JustifyContent.Start);
                break;
            case "flex-end":
                panel.SetCurrentValue(LayoutEx.JustifyContentProperty, JustifyContent.End);
                break;
            case "space-equal":
                panel.SetCurrentValue(LayoutEx.JustifyContentProperty, JustifyContent.SpaceEqual);
                break;
            case "space-between":
                panel.SetCurrentValue(LayoutEx.JustifyContentProperty, JustifyContent.SpaceBetween);
                break;
            case "space-around":
                panel.SetCurrentValue(LayoutEx.JustifyContentProperty, JustifyContent.SpaceAround);
                break;
            default:
                panel.TryApply<JustifyContent>(LayoutEx.JustifyContentProperty, value);
                break;
        }
    }

    public static void ApplyAlignContent(this MagicPanel panel, string value)
    {
        // align-content: flex-start | flex-end | center | space-between | space-around | stretch;
        switch (value)
        {
            case "flex-start":
                panel.SetCurrentValue(LayoutEx.AlignContentProperty, AlignContent.Start);
                break;
            case "flex-end":
                panel.SetCurrentValue(LayoutEx.AlignContentProperty, AlignContent.End);
                break;
            case "space-equal":
                panel.SetCurrentValue(LayoutEx.AlignContentProperty, AlignContent.SpaceEqual);
                break;
            case "space-between":
                panel.SetCurrentValue(LayoutEx.AlignContentProperty, AlignContent.SpaceBetween);
                break;
            case "space-around":
                panel.SetCurrentValue(LayoutEx.AlignContentProperty, AlignContent.SpaceAround);
                break;
            default:
                panel.TryApply<AlignContent>(LayoutEx.AlignContentProperty, value);
                break;
        }
    }
    
    public static void ApplyOrientation(this MagicPanel panel, string value)
    {
        // avalonia-orientation: h | horizontal | v | vertical 
        // flex-direction: row | row-reverse | column | column-reverse;
        switch (value)
        {
            case "h" or "H":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Horizontal);
                break;
            case "v" or "V":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Vertical);
                break;
            case "row":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Horizontal);
                panel.ApplyReverse("false");
                break;
            case "column":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Vertical);
                panel.ApplyReverse("false");
                break;
            case "row-reverse":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Horizontal);
                panel.ApplyReverse("true");
                break;
            case "column-reverse":
                panel.SetCurrentValue(LayoutEx.OrientationProperty, Orientation.Vertical);
                panel.ApplyReverse("true");
                break;
            default:
            {
                panel.TryApply<Orientation>(LayoutEx.OrientationProperty, value);
                break;
            }
        }
    }

    public static void ApplyReverse(this MagicPanel panel, string value)
    {
        var reverse = string.IsNullOrEmpty(value) || (bool.TryParse(value, out var b) && b);
        panel.SetCurrentValue(LayoutEx.ReverseProperty, reverse);
    }

    public static void ApplyFlexWrap(this MagicPanel panel, string value)
    {
        // avalonia-wrap: wrap | nowrap | wrapreverse;
        // flex-wrap: nowrap | wrap | wrap-reverse;
        switch (value)
        {
            case "wrap-reverse":
                panel.SetCurrentValue(LayoutEx.FlexWrapProperty, FlexWrap.WrapReverse);
                break;
            default:
                panel.TryApply<FlexWrap>(LayoutEx.FlexWrapProperty, value);
                break;
        }
    }
}