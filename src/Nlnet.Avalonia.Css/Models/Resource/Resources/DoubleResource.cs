using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Double))]
internal class DoubleResource : AcssResourceBaseAndFac<DoubleResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        if (double.TryParse(valueString, out var doubleValue))
        {
            return doubleValue;
        }

        this.WriteError($"Can not parse {nameof(Double)} from string '{valueString}'.");
        return null;
    }
}