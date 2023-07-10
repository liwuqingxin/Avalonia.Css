using Avalonia;

namespace Nlnet.Avalonia.Css.Controls;

public enum TabStripItemsPanelStyle
{
    StackPanel,
    WrapPanel,
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
        .RegisterAttached<TabStripItemsPanelStyles, Visual, TabStripItemsPanelStyle>("Style", TabStripItemsPanelStyle.StackPanel);
}