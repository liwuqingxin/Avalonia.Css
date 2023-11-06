using System;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Double))]
internal class DoubleResource : AcssResourceBaseAndFac<DoubleResource>
{
    protected override object? BuildValue(IAcssContext context, string valueString)
    {
        return valueString.TryParseDouble();
    }
}