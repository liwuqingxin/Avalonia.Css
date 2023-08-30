using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css.Controls;

public class ExpandingDecorator : Decorator
{
    public double Percent
    {
        get { return GetValue(PercentProperty); }
        set { SetValue(PercentProperty, value); }
    }
    public static readonly StyledProperty<double> PercentProperty = AvaloniaProperty
        .Register<ExpandingDecorator, double>(nameof(Percent));

    static ExpandingDecorator()
    {
        AffectsRender<ExpandingDecorator>(PercentProperty);
        PercentProperty.Changed.AddClassHandler<ExpandingDecorator>(OnPercentChanged);
    }

    private static void OnPercentChanged(ExpandingDecorator decorator, AvaloniaPropertyChangedEventArgs arg)
    {
        var rate = decorator.Percent / 100;
        var rect = decorator.Bounds.Inflate(50);
        rect           = rect.WithHeight(rect.Height * rate);
        decorator.Clip = new RectangleGeometry(rect);
    }
}