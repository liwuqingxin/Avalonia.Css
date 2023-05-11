using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Nlnet.Avalonia.Css;

[ResourceType(typeof(Brush))]
public class BrushResource : CssResource<BrushResource>
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
        var opacity     = 1d;
        if (values.Length >= 2 && double.TryParse(values[1], out var o))
        {
            opacity = o;
        }

        if (InterpreterHelper.IsVar(colorString, out var key))
        {
            _opacity   = opacity;
            _key       = key;

            IsDeferred = true;

            return null;
        }
        else
        {
            var color = TryParseColor(colorString);
            if (color == null)
            {
                return null;
            }

            return new ImmutableSolidColorBrush(color.Value, opacity);
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

    private static Color? TryParseColor(string colorString)
    {
        try
        {
            return Color.Parse(colorString);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
