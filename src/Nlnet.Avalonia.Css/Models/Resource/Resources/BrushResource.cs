using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Brush))]
internal class BrushResource : AcssResourceBaseAndFac<BrushResource>
{
    private double  _opacity;
    private string? _key;

    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            this.WriteError($"Can not parse {nameof(Brush)} from string '{valueString}'.");
            return null;
        }

        var colorString = values[0];
        _opacity = 1d;
        if (values.Length >= 2 && double.TryParse(values[1], out var o))
        {
            _opacity = o;
        }

        if (acssBuilder.Interpreter.IsVar(colorString, out var key))
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
                this.WriteError($"Can not parse {nameof(Brush)} from string '{valueString}'.");
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
        else
        {
            this.WriteError($"Can not find the resource with key '{_key}'.");
        }
        return brush;
    }
}
