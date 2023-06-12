using Avalonia;

namespace Nlnet.Avalonia.Css.Fluent;

public enum TabStripItemsPanelStyle
{
    StackPanel,
    WarpPanel,
}

public class TabStripItemsPanelStyles : AvaloniaObject
{
    public static TabStripItemsPanelStyle GetStyle(Visual host)
    {
        return host.GetValue(StyleProperty);
    }
    public static void SetStyle(Visual host, TabStripItemsPanelStyle value)
    {
        host.SetValue(StyleProperty, value);
    }
    public static readonly AttachedProperty<TabStripItemsPanelStyle> StyleProperty = AvaloniaProperty
        .RegisterAttached<TabStripItemsPanelStyles, Visual, TabStripItemsPanelStyle>("Style");
}
