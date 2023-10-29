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
    public ResourceDictionary? ToAvaloniaResourceDictionary();

    public bool IsThemeResource();

    public ThemeVariant GetThemeVariant();
}

internal class AcssResourceDictionary : AcssSection, IAcssResourceDictionary
{
    private readonly IAcssContext _context;

    private static readonly Regex RegexAccent      = new("\\[accent=(.*?)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexTheme       = new("\\[theme=(.*?)\\]", RegexOptions.IgnoreCase);
    private static readonly Regex RegexDescription = new("\\[desc=(.*?)\\]", RegexOptions.IgnoreCase);

    public string? Accent { get; set; }
    
    public string? Theme { get; set; }

    public string? Description { get; set; }

    public List<AcssResource> Resources { get; set; } = new();

    public AcssResourceDictionary(IAcssContext context, string selector) : base(context, selector)
    {
        _context = context;
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

    public override IAcssSection Clone()
    {
        throw new NotImplementedException();
    }

    public ResourceDictionary? ToAvaloniaResourceDictionary()
    {
        if (Resources.Count == 0)
        {
            this.WriteWarning($"No resource detected. Skip this.");
            return null;
        }

        var cfg = _context.GetService<IAcssConfiguration>();
        if (Accent != null && !string.Equals(Accent, cfg.Accent, StringComparison.CurrentCultureIgnoreCase))
        {
            this.WriteWarning($"Current theme is '{cfg.Accent}'. This theme is '{Accent}'. Skip this.");
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
                dic.AddDeferred(resource.Key, provider => resource.BuildDeferredValue(_context, provider));
            }
            else
            {
                dic.TryAdd(resource.Key, resource.BuildValue(_context));
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
        var parser = _context.GetService<IAcssParser>();
        var resFactory = _context.GetService<IAcssResourceFactory>();
        var list = parser.ParsePairs(resources);
        foreach (var pair in list)
        {
            if (string.IsNullOrWhiteSpace(pair.Item1) || string.IsNullOrWhiteSpace(pair.Item2))
            {
                continue;
            }
            if (resFactory.TryGetResourceInstance(pair.Item1, pair.Item2, out var acssResource))
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
