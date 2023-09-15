using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface IAcssResourceDictionary : IAcssSection
{
    public ResourceDictionary? ToAvaloniaResourceDictionary(IAcssBuilder acssBuilder);

    public bool IsThemeResource();

    public ThemeVariant GetThemeVariant();
}

internal class AcssResourceDictionary : AcssSection, IAcssResourceDictionary
{
    private readonly IAcssBuilder _builder;

    private static readonly Regex RegexAccent      = new("\\[accent=(.*?)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexTheme       = new("\\[theme=(.*?)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexDescription = new("\\[desc=(.*?)\\]", RegexOptions.IgnoreCase);

    public string? Accent { get; set; }
    
    public string? Theme { get; set; }

    public string? Description { get; set; }

    public List<AcssResource> Resources { get; set; } = new();

    public AcssResourceDictionary(IAcssBuilder builder, string selector) : base(builder, selector)
    {
        _builder = builder;
    }

    public override void InitialSection(IAcssParser parser, ReadOnlySpan<char> content)
    {
        var matchAccent = RegexAccent.Match(Header);
        var matchTheme  = RegexTheme.Match(Header);
        var matchDesc   = RegexDescription.Match(Header);
        if (matchAccent.Success)
        {
            Accent = matchAccent.Groups[1].Value;
        }
        if (matchTheme.Success)
        {
            Theme = matchTheme.Groups[1].Value;
        }
        if (matchDesc.Success)
        {
            Description = matchDesc.Groups[1].Value;
        }

        Resources.AddRange(TryGetResources(content.ToString()).ToList());
    }

    public ResourceDictionary? ToAvaloniaResourceDictionary(IAcssBuilder acssBuilder)
    {
        if (Resources.Count == 0)
        {
            this.WriteWarning($"No resource detected. Skip this.");
            return null;
        }

        if (Accent != null && !string.Equals(Accent, acssBuilder.Configuration.Theme, StringComparison.CurrentCultureIgnoreCase))
        {
            this.WriteWarning($"Current theme is '{acssBuilder.Configuration.Theme}'. This theme is '{Accent}'. Skip this.");
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
                dic.AddDeferred(resource.Key, provider => resource.BuildDeferredValue(_builder, provider));
            }
            else
            {
                dic.TryAdd(resource.Key, resource.BuildValue(_builder));
            }
        }

        return dic;
    }

    public ThemeVariant GetThemeVariant()
    {
        switch (Theme)
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

    public bool IsThemeResource()
    {
        return string.IsNullOrEmpty(Theme) == false;
    }

    private IEnumerable<AcssResource> TryGetResources(string resources)
    {
        var list = _builder.Parser.ParsePairs(resources);
        foreach (var pair in list)
        {
            if (string.IsNullOrWhiteSpace(pair.Item1) || string.IsNullOrWhiteSpace(pair.Item2))
            {
                continue;
            }
            if (_builder.ResourceFactory.TryGetResourceInstance(pair.Item1, pair.Item2, out var acssResource))
            {
                yield return acssResource!;
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
        if (string.IsNullOrEmpty(Theme) == false)
        {
            builder.Append($"[Theme:{Theme}]");
        }
        if (string.IsNullOrEmpty(Accent) == false)
        {
            builder.Append($"[Accent:{Accent}]");
        }
        return builder.ToString();
    }
}
