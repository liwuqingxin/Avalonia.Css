using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Color))]
public class ColorResource : CssResource<ColorResource>
{
    protected override object? Accept(string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            return null;
        }

        var colorString = values[0];
        var color       = TryParseColor(colorString);

        return color;
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