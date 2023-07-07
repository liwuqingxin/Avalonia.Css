using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace Nlnet.Avalonia.SampleAssistant;

public class GalleryItemControl : ContentControl, IStyleable, IXamlProvider
{
    Type IStyleable.StyleKey => typeof(GalleryItemControl);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        Opacity = 1;
        RenderTransform = TransformOperations.Parse("none");
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        Opacity = 0;
        RenderTransform = TransformOperations.Parse("translate(0,15px)");
    }

    public string? ProvideXaml(string key)
    {
        return DataContext is not IXamlProvider xamlProvider ? null : xamlProvider.ProvideXaml(key);
    }
}
