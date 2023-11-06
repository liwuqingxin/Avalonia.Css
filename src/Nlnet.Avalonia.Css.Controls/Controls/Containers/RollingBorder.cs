using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css.Controls;

public class RollingBorder : Border
{
    public double Percent
    {
        get { return GetValue(PercentProperty); }
        set { SetValue(PercentProperty, value); }
    }
    public static readonly StyledProperty<double> PercentProperty = AvaloniaProperty
        .Register<RollingBorder, double>(nameof(Percent), 100);

    static RollingBorder()
    {
        AffectsRender<RollingBorder>(PercentProperty);
        PercentProperty.Changed.AddClassHandler<RollingBorder>(OnPercentChanged);
    }

    private static void OnPercentChanged(RollingBorder decorator, AvaloniaPropertyChangedEventArgs arg)
    {
        decorator.UpdateClip();
    }

    private void UpdateClip()
    {
        var rate = this.Percent / 100;
        var rect = this.Bounds.Inflate(50);
        rect = rect.WithHeight(rect.Height * rate);
        this.Clip = new RectangleGeometry(rect);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateClip();
    }
}