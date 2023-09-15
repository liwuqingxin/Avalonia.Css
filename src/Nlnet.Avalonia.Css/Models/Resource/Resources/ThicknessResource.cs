using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Thickness))]
[ResourceType("Thick")]
[ResourceType("StrokeThickness")]
[ResourceType("Margin")]
[ResourceType("Padding")]
internal class ThicknessResource : AcssResourceBaseAndFac<ThicknessResource>
{
    protected override object? BuildValue(IAcssBuilder acssBuilder, string valueString)
    {
        return valueString.TryParseThickness();
    }
}
