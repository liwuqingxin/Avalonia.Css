using Avalonia;

namespace Nlnet.Avalonia.Css.Fluent;

public class TabItemExtension : AvaloniaObject
{
    public static object? GetIconContent(Visual host)
    {
        return host.GetValue(IconContentProperty);
    }
    public static void SetIconContent(Visual host, object? value)
    {
        host.SetValue(IconContentProperty, value);
    }
    public static readonly AttachedProperty<object?> IconContentProperty = AvaloniaProperty
        .RegisterAttached<TabItemExtension, Visual, object?>("IconContent");
}
