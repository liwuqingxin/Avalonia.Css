using Avalonia;
using Avalonia.Layout;

namespace Nlnet.Avalonia;

public class LayoutEx : AvaloniaObject
{
    // ArrangedWidth
    public static double GetArrangedWidth(Layoutable host)
    {
        return host.GetValue(ArrangedWidthProperty);
    }
    internal static void SetArrangedWidth(Layoutable host, double value)
    {
        host.SetValue(ArrangedWidthProperty, value);
    }
    public static readonly AttachedProperty<double> ArrangedWidthProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, Layoutable, double>("ArrangedWidth", double.NaN);

    // ArrangedHeight
    public static double GetArrangedHeight(Layoutable host)
    {
        return host.GetValue(ArrangedHeightProperty);
    }
    internal static void SetArrangedHeight(Layoutable host, double value)
    {
        host.SetValue(ArrangedHeightProperty, value);
    }
    public static readonly AttachedProperty<double> ArrangedHeightProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, Layoutable, double>("ArrangedHeight", double.NaN);

    // ArrangedLeft
    public static double GetArrangedLeft(Layoutable host)
    {
        return host.GetValue(ArrangedLeftProperty);
    }
    public static void SetArrangedLeft(Layoutable host, double value)
    {
        host.SetValue(ArrangedLeftProperty, value);
    }
    public static readonly AttachedProperty<double> ArrangedLeftProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, Layoutable, double>("ArrangedLeft");

    // ArrangedTop
    public static double GetArrangedTop(Layoutable host)
    {
        return host.GetValue(ArrangedTopProperty);
    }
    public static void SetArrangedTop(Layoutable host, double value)
    {
        host.SetValue(ArrangedTopProperty, value);
    }
    public static readonly AttachedProperty<double> ArrangedTopProperty = AvaloniaProperty
        .RegisterAttached<LayoutEx, Layoutable, double>("ArrangedTop");
}