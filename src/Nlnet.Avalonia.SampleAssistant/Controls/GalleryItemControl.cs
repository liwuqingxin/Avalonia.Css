using System;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.SampleAssistant;

public class GalleryItemControl : ContentControl, IStyleable, IXamlProvider
{
    Type IStyleable.StyleKey => typeof(GalleryItemControl);

    public string? ProvideXaml(string key)
    {
        return DataContext is not IXamlProvider xamlProvider ? null : xamlProvider.ProvideXaml(key);
    }
}
