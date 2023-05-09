using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Nlnet.Avalonia.Css;

public class BrushResource : CssResource<BrushResource>
{
    protected override object? Accept(string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            return null;
        }

        var colorString = values[0];
        var opacity     = 1d;
        if (values.Length >= 2 && double.TryParse(values[1], out var o))
        {
            opacity = o;
        }

        if (IsVar(colorString, out var key))
        {
            var dyn   = new DynamicResourceExtension(key!);
            var brush = new SolidColorBrush
            {
                Opacity = opacity
            };
            brush.Bind(SolidColorBrush.ColorProperty, dyn);
            return brush;
        }
        else
        {
            var color = TryParseColor(colorString);
            if (color == null)
            {
                return null;
            }

            return new ImmutableSolidColorBrush(color.Value, opacity);
        }
    }

    private static Color? TryParseColor(string colorString)
    {
        try
        {
            return Color.Parse(colorString);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}


public class CssResources
{
    private static readonly Regex Regex = new(":res(\\[(theme=.*)\\]|\\[(mode=.*)\\])*", RegexOptions.IgnoreCase);

    public string? Theme { get; set; }
    
    public string? Mode  { get; set; }

    public List<CssResource> Resources { get; set; } = new();

    public bool TryGetResource(string selector, string content)
    {
        var match = Regex.Match(selector);

    }
}
