using System;
using System.Collections.Generic;
using System.Diagnostics;
// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.SampleAssistant;

public class GalleryItem : IXamlProvider
{
    private readonly Dictionary<string, string>? _caseXamlDictionary;

    public string          Title        { get; set; }
    public string?         Icon         { get; set; }
    public Type            ViewType     { get; set; }
    public GalleryItemKind ViewItemKind { get; set; }
    public string?         Description  { get; set; }
    public string?         Xaml         { get; set; }
    
    [Obsolete("Just for Design.DataContext.")]
    public GalleryItem()
    {
        Title    = "";
        ViewType = typeof(GalleryItem);
    }

    public GalleryItem(string? title, string? icon, Type viewType, GalleryItemKind viewItemKind, string? description, string? xaml)
    {
        Title        = title ?? string.Empty;
        Icon         = icon;
        ViewType     = viewType;
        ViewItemKind = viewItemKind;
        Description  = description;
        Xaml         = xaml;

        try
        {
            _caseXamlDictionary = LoadService.XmlParser.ParseCases(xaml);
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
    }

    public string? ProvideXaml(string key)
    {
        if (_caseXamlDictionary == null)
        {
            return null;
        }

        return _caseXamlDictionary.TryGetValue(key, out var value) ? value : null;
    }
}
