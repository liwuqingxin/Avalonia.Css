using System;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Double))]
internal class DoubleResource : AcssResourceBaseAndFac<DoubleResource>
{
    protected override object? BuildValue(IAcssBuilder acssBuilder, string valueString)
    {
        return valueString.TryParseDouble();
    }
}