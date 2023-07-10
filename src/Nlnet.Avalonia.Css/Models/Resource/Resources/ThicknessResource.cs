using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Thickness))]
[ResourceType("StrokeThickness")]
[ResourceType("Margin")]
[ResourceType("Padding")]
internal class ThicknessResource : CssResourceBaseAndFac<ThicknessResource>
{
    protected override object? Accept(string valueString)
    {
        try
        {
            return Thickness.Parse(valueString);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
