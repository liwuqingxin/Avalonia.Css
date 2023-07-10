using Avalonia;

namespace Nlnet.Avalonia.Css.Controls;

public class SliderExtension : AvaloniaObject
{
    public static string GetFormat(Visual host)
    {
        return host.GetValue(FormatProperty);
    }
    public static void SetFormat(Visual host, string value)
    {
        host.SetValue(FormatProperty, value);
    }
    public static readonly AttachedProperty<string> FormatProperty = AvaloniaProperty
        .RegisterAttached<SliderExtension, Visual, string>("Format", "{0:F0}");
}
