using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Thickness))]
[ResourceType("StrokeThickness")]
[ResourceType("Margin")]
[ResourceType("Padding")]
internal class ThicknessResource : AcssResourceBaseAndFac<ThicknessResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
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
