﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Brush))]
public class BrushResource : CssResourceBaseAndFac<BrushResource>
{
    private double  _opacity;
    private string? _key;

    protected override object? Accept(string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            return null;
        }

        var colorString = values[0];
        _opacity = 1d;
        if (values.Length >= 2 && double.TryParse(values[1], out var o))
        {
            _opacity = o;
        }

        if (CssServiceLocator.GetService<ICssInterpreter>().IsVar(colorString, out var key))
        {
            _key = key;

            IsDeferred = true;

            return null;
        }
        else
        {
            var color = DataParser.TryParseColor(colorString);
            if (color == null)
            {
                return null;
            }

            return new ImmutableSolidColorBrush(color.Value, _opacity);
        }
    }

    public override object? GetDeferredValue(IServiceProvider? provider)
    {
        var brush = new SolidColorBrush
        {
            Opacity = _opacity,
        };

        if (Application.Current != null && Application.Current.TryFindResource(_key!, out var value) && value is Color c)
        {
            brush.Color = c;
        }
        return brush;
    }
}
