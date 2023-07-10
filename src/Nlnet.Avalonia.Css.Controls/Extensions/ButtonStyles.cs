using Avalonia;

namespace Nlnet.Avalonia.Css.Controls;

public enum ButtonStyle
{
    Normal,
    Accent,
}

public class ButtonStyles : AvaloniaObject
{
    public static ButtonStyle GetStyle(Visual host)
    {
        return host.GetValue(StyleProperty);
    }
    public static void SetStyle(Visual host, ButtonStyle value)
    {
        host.SetValue(StyleProperty, value);
    }
    public static readonly AttachedProperty<ButtonStyle> StyleProperty = AvaloniaProperty
        .RegisterAttached<ButtonStyles, Visual, ButtonStyle>("Style");
}