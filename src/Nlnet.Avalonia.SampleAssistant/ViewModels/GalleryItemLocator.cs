using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Nlnet.Avalonia.SampleAssistant;

public class GalleryItemLocator : IDataTemplate
{
    private readonly Dictionary<Type, IControl> _cache = new();

    public IControl Build(object? data)
    {
        if (data is not GalleryItem item)
        {
            return new TextBlock {Text = $"View Not Found: {data}"};
        }

        if (_cache.TryGetValue(item.ViewType, out var ctrl))
        {
            return ctrl;
        }

        var view = (Activator.CreateInstance(item.ViewType) as IControl)!;
        var galleryItemControl = new GalleryItemControl()
        {
            Content = view,
        };

        _cache.Add(item.ViewType, galleryItemControl);

        return galleryItemControl;
    }

    public bool Match(object? data)
    {
        return data is GalleryItem;
    }
}
