using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Color))]
internal class ColorResource : CssResourceBaseAndFac<ColorResource>
{
    protected override object? Accept(ICssBuilder cssBuilder, string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            return null;
        }

        var colorString = values[0];
        var color       = DataParser.TryParseColor(colorString);

        return color;
    }
}