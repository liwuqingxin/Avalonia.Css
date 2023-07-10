using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Double))]
internal class DoubleResource : CssResourceBaseAndFac<DoubleResource>
{
    protected override object? Accept(ICssBuilder cssBuilder, string valueString)
    {
        if (double.TryParse(valueString, out var doubleValue))
        {
            return doubleValue;
        }

        return null;
    }
}