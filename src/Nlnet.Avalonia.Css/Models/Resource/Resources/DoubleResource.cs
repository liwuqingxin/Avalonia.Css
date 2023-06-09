using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Double))]
public class DoubleResource : CssResource<DoubleResource>
{
    protected override object? Accept(string valueString)
    {
        if (double.TryParse(valueString, out var doubleValue))
        {
            return doubleValue;
        }

        return null;
    }
}