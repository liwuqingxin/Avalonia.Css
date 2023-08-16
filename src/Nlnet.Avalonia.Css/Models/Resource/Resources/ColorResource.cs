using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Color))]
internal class ColorResource : AcssResourceBaseAndFac<ColorResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            this.WriteError($"Can not parse {nameof(Color)} from string '{valueString}'.");
            return null;
        }

        var colorString = values[0];
        var color       = DataParser.TryParseColor(colorString);

        return color;
    }
}