using System;
using System.Collections.Generic;
using System.Diagnostics;
// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.SampleAssistant;

public class GalleryItem : IXamlProvider
{
    private readonly ICaseXamlParser _xmlParser = new XCaseXamlParser();
    private readonly Dictionary<string, string>? _caseXamlDictionary;

    public string       Title        { get; }
    public string?      Icon         { get; }
    public Type         ViewType     { get; }
    public ViewItemKind ViewItemKind { get; }
    public string?      Xaml         { get; }

    public GalleryItem(string? title, string? icon, Type viewType, ViewItemKind viewItemKind, string? xaml)
    {
        Title        = title ?? string.Empty;
        Icon         = icon;
        ViewType     = viewType;
        ViewItemKind = viewItemKind;
        Xaml         = xaml;

        try
        {
            _caseXamlDictionary = _xmlParser.ParseCases(xaml);
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
