using System;
using Avalonia;
using Avalonia.Layout;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public class LinearPanel : ReactivePanel
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
        .RegisterAttached<LinearPanel, MagicPanel, Orientation>("Orientation", Orientation.Vertical);
    
    
    
    protected static void ApplyOrientation(MagicPanel panel, string value)
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
}