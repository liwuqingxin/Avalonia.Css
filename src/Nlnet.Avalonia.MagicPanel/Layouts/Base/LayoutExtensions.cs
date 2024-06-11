using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

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

        panel.SetCurrentValue(MagicPanel.SpacingProperty, v);
    }

    public static void ApplyAlignItems(this MagicPanel panel, string value)
    {
        // align-items: flex-start | flex-end | center | baseline | stretch;
        switch (value)
        {
            case "flex-start":
                panel.SetCurrentValue(MagicPanel.AlignItemsProperty, Alignment.Start);
                break;
            case "flex-end":
                panel.SetCurrentValue(MagicPanel.AlignItemsProperty, Alignment.End);
                break;
            case "baseline":
                Trace.WriteLine("The baseline for align-items is not supported.");
                break;
            default:
                panel.TryApply<Alignment>(MagicPanel.AlignItemsProperty, value);
                break;
        }
    }

    public static void ApplyJustifyContent(this MagicPanel panel, string value)
    {
        // justify-content: flex-start | flex-end | center | space-between | space-around;
        switch (value)
        {
            case "flex-start":
                panel.SetCurrentValue(MagicPanel.JustifyContentProperty, JustifyContent.Start);
                break;
            case "flex-end":
                panel.SetCurrentValue(MagicPanel.JustifyContentProperty, JustifyContent.End);
                break;
            case "space-evenly":
                panel.SetCurrentValue(MagicPanel.JustifyContentProperty, JustifyContent.SpaceEvenly);
                break;
            case "space-between":
                panel.SetCurrentValue(MagicPanel.JustifyContentProperty, JustifyContent.SpaceBetween);
                break;
            case "space-around":
                panel.SetCurrentValue(MagicPanel.JustifyContentProperty, JustifyContent.SpaceAround);
                break;
            default:
                panel.TryApply<JustifyContent>(MagicPanel.JustifyContentProperty, value);
                break;
        }
    }

    public static void ApplyAlignContent(this MagicPanel panel, string value)
    {
        // align-content: flex-start | flex-end | center | space-between | space-around | stretch;
        switch (value)
        {
            case "flex-start":
                panel.SetCurrentValue(MagicPanel.AlignContentProperty, AlignContent.Start);
                break;
            case "flex-end":
                panel.SetCurrentValue(MagicPanel.AlignContentProperty, AlignContent.End);
                break;
            case "space-evenly":
                panel.SetCurrentValue(MagicPanel.AlignContentProperty, AlignContent.SpaceEvenly);
                break;
            case "space-between":
                panel.SetCurrentValue(MagicPanel.AlignContentProperty, AlignContent.SpaceBetween);
                break;
            case "space-around":
                panel.SetCurrentValue(MagicPanel.AlignContentProperty, AlignContent.SpaceAround);
                break;
            default:
                panel.TryApply<AlignContent>(MagicPanel.AlignContentProperty, value);
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
                panel.SetCurrentValue(MagicPanel.OrientationProperty, Orientation.Horizontal);
                break;
            case "v" or "V":
                panel.SetCurrentValue(MagicPanel.OrientationProperty, Orientation.Vertical);
                break;
            case "row":
                panel.SetCurrentValue(MagicPanel.OrientationProperty, Orientation.Horizontal);
                panel.ApplyReverse("false");
                break;
            case "column":
                panel.SetCurrentValue(MagicPanel.OrientationProperty, Orientation.Vertical);
                panel.ApplyReverse("false");
                break;
            case "row-reverse":
                panel.SetCurrentValue(MagicPanel.OrientationProperty, Orientation.Horizontal);
                panel.ApplyReverse("true");
                break;
            case "column-reverse":
                panel.SetCurrentValue(MagicPanel.OrientationProperty, Orientation.Vertical);
                panel.ApplyReverse("true");
                break;
            default:
            {
                panel.TryApply<Orientation>(MagicPanel.OrientationProperty, value);
                break;
            }
        }
    }

    public static void ApplyReverse(this MagicPanel panel, string value)
    {
        var reverse = string.IsNullOrEmpty(value) || (bool.TryParse(value, out var b) && b);
        panel.SetCurrentValue(MagicPanel.ReverseProperty, reverse);
    }

    public static void ApplyFlexWrap(this MagicPanel panel, string value)
    {
        // avalonia-wrap: wrap | nowrap | wrapreverse;
        // flex-wrap: nowrap | wrap | wrap-reverse;
        switch (value)
        {
            case "wrap-reverse":
                panel.SetCurrentValue(MagicPanel.FlexWrapProperty, FlexWrap.WrapReverse);
                break;
            default:
                panel.TryApply<FlexWrap>(MagicPanel.FlexWrapProperty, value);
                break;
        }
    }
}