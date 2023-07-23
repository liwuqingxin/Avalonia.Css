﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface ICssResourceDictionary : ICssSection
{
    public ResourceDictionary? ToAvaloniaResourceDictionary(ICssBuilder cssBuilder);

    public bool IsModeResource();

    public ThemeVariant GetThemeVariant();
}

internal class CssResourceDictionary : CssSection, ICssResourceDictionary
{
    private readonly ICssBuilder _builder;

    private static readonly Regex RegexTheme       = new("\\[theme=(.*?)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexMode        = new("\\[mode=(.*?)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexDescription = new("\\[desc=(.*?)\\]", RegexOptions.IgnoreCase);

    public string? Theme { get; set; }
    
    public string? Mode { get; set; }

    public string? Description { get; set; }

    public List<CssResource> Resources { get; set; } = new();

    public CssResourceDictionary(ICssBuilder builder, string selector) : base(builder, selector)
    {
        _builder = builder;
    }

    public override void InitialSection(ICssParser parser, ReadOnlySpan<char> content)
    {
        var matchTheme = RegexTheme.Match(Selector);
        var matchMode  = RegexMode.Match(Selector);
        var matchDesc  = RegexDescription.Match(Selector);
        if (matchTheme.Success)
        {
            Theme = matchTheme.Groups[1].Value;
        }
        if (matchMode.Success)
        {
            Mode = matchMode.Groups[1].Value;
        }
        if (matchDesc.Success)
        {
            Description = matchDesc.Groups[1].Value;
        }

        Resources.AddRange(TryGetResources(content.ToString()).ToList());
    }

    public ResourceDictionary? ToAvaloniaResourceDictionary(ICssBuilder cssBuilder)
    {
        if (Resources.Count == 0)
        {
            this.WriteWarning($"No resource detected. Skip this.");
            return null;
        }

        if (Theme != null && !string.Equals(Theme, cssBuilder.Configuration.Theme, StringComparison.CurrentCultureIgnoreCase))
        {
            this.WriteWarning($"Current theme is '{cssBuilder.Configuration.Theme}'. This theme is '{Theme}'. Skip this.");
            return null;
        }

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

        return dic;
    }

    public ThemeVariant GetThemeVariant()
    {
        switch (Mode)
        {
            case "Dark":
            case "dark":
                return ThemeVariant.Dark;
            case "Light":
            case "light":
                return ThemeVariant.Light;
            default:
                return ThemeVariant.Default;
        }
    }

    public bool IsModeResource()
    {
        return string.IsNullOrEmpty(Mode) == false;
    }

    private IEnumerable<CssResource> TryGetResources(string resources)
    {
        resources = resources.ReplaceLineEndings(" ");
        var resourceList = resources.Split(";", StringSplitOptions.RemoveEmptyEntries);
        foreach (var resource in resourceList)
        {
            var r = resource.Trim().TrimEnd(';');
            if (string.IsNullOrWhiteSpace(r))
            {
                continue;
            }
            if (_builder.ResourceFactory.TryGetResourceInstance(resource, out var cssResource))
            {
                yield return cssResource!;
            }
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        if (string.IsNullOrEmpty(Description) == false)
        {
            builder.Append($"[{Description}]");
        }
        if (string.IsNullOrEmpty(Mode) == false)
        {
            builder.Append($"[Mode:{Mode}]");
        }
        if (string.IsNullOrEmpty(Theme) == false)
        {
            builder.Append($"[Theme:{Theme}]");
        }
        return builder.ToString();
    }
}
