using System;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Color))]
internal class ColorResource : AcssResourceBaseAndFac<ColorResource>
{
    protected override object? BuildValue(IAcssContext context, string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            context.OnError(AcssErrors.Value_String_Invalid, $"Can not parse {nameof(Color)} from string '{valueString}'.");
            return null;
        }

        var colorString = values[0];
        var color       = colorString.TryParseColor();
        if (color != null && values.Length > 1 && double.TryParse(values[1], out var o))
        {
            color = color.Value.ApplyOpacity(o);
        }

        return color;
    }
}