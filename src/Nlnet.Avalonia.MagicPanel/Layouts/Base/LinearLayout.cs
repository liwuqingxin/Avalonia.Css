using System;
using Avalonia;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public abstract class LinearLayout : ReactiveLayout
{
    public static Orientation GetOrientation(MagicPanel host)
    {
        return host.GetValue(OrientationProperty);
    }
    public static void SetOrientation(MagicPanel host, Orientation value)
    {
        host.SetValue(OrientationProperty, value);
    }
    public static readonly AttachedProperty<Orientation> OrientationProperty = AvaloniaProperty
        .RegisterAttached<LinearLayout, MagicPanel, Orientation>("Orientation", Orientation.Vertical);

    public static bool GetReverse(MagicPanel host)
    {
        return host.GetValue(ReverseProperty);
    }
    public static void SetReverse(MagicPanel host, bool value)
    {
        host.SetValue(ReverseProperty, value);
    }
    public static readonly AttachedProperty<bool> ReverseProperty = AvaloniaProperty
        .RegisterAttached<LinearLayout, MagicPanel, bool>("Reverse");


    
    #region IMagicLayout
    
    public override void ApplySetter(MagicPanel panel, string property, string value)
    {
        base.ApplySetter(panel, property, value);
        
        switch (property)
        {
            case "orientation":
            case "direction":
            {
                ApplyOrientation(panel, value);
                break;
            }
            case "r":
            case "reverse":
            {
                ApplyReverse(panel, value);
                break;
            }
        }
    }
    
    private static void ApplyOrientation(MagicPanel panel, string value)
    {
        switch (value)
        {
            case "v" or "V":
                panel.SetCurrentValue(OrientationProperty, Orientation.Vertical);
                break;
            case "h" or "H":
                panel.SetCurrentValue(OrientationProperty, Orientation.Horizontal);
                break;
            default:
            {
                if (Enum.TryParse<Orientation>(value, true, out var v) == false)
                {
                    return;
                }

                panel.SetCurrentValue(OrientationProperty, v);
                break;
            }
        }
    }

    private static void ApplyReverse(MagicPanel panel, string value)
    {
        var reverse = string.IsNullOrEmpty(value) || (bool.TryParse(value, out var b) && b);
        panel.SetCurrentValue(ReverseProperty, reverse);
    }

    #endregion
}