using Avalonia;

namespace Nlnet.Avalonia.Css.Controls;

public class ButtonStyles : AvaloniaObject
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
        .RegisterAttached<ButtonStyles, Visual, string?>("Style");
}