using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType("Linear")]
[ResourceType(nameof(LinearGradientBrush))]
internal class LinearResource : AcssResourceBaseAndFac<LinearResource>
{
    private LinearGradientBrush? _brush;
    private Queue<(string,double)>?     _keys;

    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        valueString = valueString.Trim();
        if (valueString.StartsWith("("))
        {
            _brush = acssBuilder.Interpreter.ParseLinear(valueString, out var shouldDefer, out var list);
            IsDeferred = shouldDefer;
            if (list != null)
            {
                _keys = new Queue<(string, double)>(list);
            }
            return _brush?.ToImmutable();
        }
        else if (valueString.StartsWith("{"))
        {
            _brush = acssBuilder.Interpreter.ParseComplexLinear(valueString, out var shouldDefer, out var list);
            IsDeferred = shouldDefer;
            if (list != null)
            {
                _keys = new Queue<(string, double)>(list);
            }
            return _brush?.ToImmutable();
        }
        else
        {
            return null;
        }
    }

    public override object? GetDeferredValue(IAcssBuilder acssBuilder, IServiceProvider? provider)
    {
        if (_brush == null || _keys == null)
        {
            return null;
        }
        
        foreach (var stop in _brush.GradientStops.Where(stop => !stop.IsSet(GradientStop.ColorProperty)))
        {
            if (_keys.TryDequeue(out var key) == false)
            {
                this.WriteError($"Unset gradient stop exist but no deferred keys provided.");
                break;
            }
            
            if (acssBuilder.ResourceProvidersManager.TryFindResource<Color>(key.Item1, out var color))
            {
                stop.Color = color.ApplyOpacity(key.Item2);
            }
            else
            {
                this.WriteError($"Can not find the resource with key '{key.Item2}'.");
            }
        }

        return _brush.ToImmutable();
    }
}