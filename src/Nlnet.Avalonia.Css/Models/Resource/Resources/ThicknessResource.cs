using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Thickness))]
[ResourceType("StrokeThickness")]
[ResourceType("Margin")]
[ResourceType("Padding")]
internal class ThicknessResource : CssResourceBaseAndFac<ThicknessResource>
{
    protected override object? Accept(IAcssBuilder cssBuilder, string valueString)
    {
        try
        {
            return Thickness.Parse(valueString);
        }
        catch
        {
            this.WriteError($"Can not parse {nameof(Thickness)} from string '{valueString}'.");
            return null;
        }
    }
}
