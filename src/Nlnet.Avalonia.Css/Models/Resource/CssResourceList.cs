using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

public class CssResourceList
{
    private static readonly Regex RegexSelector = new(":res(\\[.*\\])?", RegexOptions.IgnoreCase);
    private static readonly Regex RegexTheme    = new("\\[theme=(.*)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexMode     = new("\\[mode=(.*)\\]", RegexOptions.IgnoreCase);

    public string? Theme { get; set; }
    
    public string? Mode { get; set; }

    public List<CssResource> Resources { get; set; } = new();

    private CssResourceList()
    {
        
    }

    public ResourceDictionary? ToResourceDictionary()
    {
        var dic = new ResourceDictionary();

        foreach (var resource in Resources)
        {
            if (resource.Key == null)
            {
                continue;
            }
            if (resource.IsDeferred)
            {
                dic.AddDeferred(resource.Key, provider => resource.GetDeferredValue(provider));
            }
            else
            {
                dic.TryAdd(resource.Key, resource.Value);
            }
        }

        if (dic.Count == 0)
        {
            return null;
        }
        return dic;
    }

    public static bool TryGetResourceList(string selector, string content, out CssResourceList? cssResources)
    {
        var match = RegexSelector.Match(selector);
        if (match.Success == false)
        {
            cssResources = null;
            return false;
        }

        var resources = new CssResourceList();

        var matchTheme = RegexTheme.Match(selector);
        var matchMode  = RegexMode.Match(selector);
        if (matchTheme.Success == false)
        {
            resources.Theme = matchTheme.Groups[1].Value;
        }
        if (matchMode.Success)
        {
            resources.Mode = matchMode.Groups[1].Value;
        }

        resources.Resources.AddRange(TryGetResources(content).ToList());

        cssResources = resources;
        return true;
    }

    private static IEnumerable<CssResource> TryGetResources(string resources)
    {
        resources = resources.ReplaceLineEndings(" ");
        var resourceList = resources.Split(";", StringSplitOptions.RemoveEmptyEntries);
        foreach (var resource in resourceList)
        {
            var r = resource.Trim().TrimEnd(';');
            if (string.IsNullOrWhiteSpace(r) != false)
            {
                continue;
            }
            if (CssResourceFactory.TryGetResourceInstance(resource, out var cssResource))
            {
                yield return cssResource!;
            }
        }
    }
}
