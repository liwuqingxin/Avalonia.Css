using System;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Double))]
internal class DoubleResource : AcssResourceBaseAndFac<DoubleResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        return valueString.TryParseDouble();
    }
}