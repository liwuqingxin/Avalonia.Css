using System;
using Avalonia.Controls;

namespace Nlnet.Avalonia.SampleAssistant;

public class GalleryItemControl : ContentControl, IXamlProvider
{
    protected override Type StyleKeyOverride { get; } = typeof(GalleryItemControl);

    public string? ProvideXaml(string key)
    {
        return DataContext is not IXamlProvider xamlProvider ? null : xamlProvider.ProvideXaml(key);
    }
}
