using Avalonia;

namespace Nlnet.Avalonia.Css.Controls;

public class PopupStyles : AvaloniaObject
{
    public static string? GetStyle(Visual host)
    {
        return host.GetValue(StyleProperty);
    }
    public static void SetStyle(Visual host, string? value)
    {
        host.SetValue(StyleProperty, value);
    }
    public static readonly AttachedProperty<string?> StyleProperty = AvaloniaProperty
        .RegisterAttached<TabStripItemsPanelStyles, Visual, string?>("Style", null, true);
}