using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Brush))]
internal class BrushResource : AcssResourceBaseAndFac<BrushResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        var values = valueString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0)
        {
            this.WriteError($"Can not parse {nameof(Brush)} from string '{valueString}'.");
            return null;
        }
        
        var colorString = values[0];
        var opacity = 1d;
        if (values.Length >= 2 && double.TryParse(values[1], out var o))
        {
            opacity = o;
        }

        if (acssBuilder.Interpreter.IsVar(colorString, out var key))
        {
            // https://github.com/AvaloniaUI/Avalonia/issues/4616
            // https://github.com/AvaloniaUI/Avalonia/discussions/12847
            // https://github.com/AvaloniaUI/Avalonia/issues/12854

            if (Application.Current == null)
            {
                throw new InvalidOperationException($"Can not create dynamic resource because Application.Current is null.");
            }
            var brush = new SolidColorBrush()
            {
                Opacity = opacity,
                [!SolidColorBrush.ColorProperty] = Application.Current.GetResourceObservable(key!).ToBinding(),
            };

            return brush;
        }

        var color = DataParser.TryParseColor(colorString);
        if (color == null)
        {
            this.WriteError($"Can not parse {nameof(Brush)} from string '{valueString}'.");
            return null;
        }

        return new ImmutableSolidColorBrush(color.Value, opacity);
    }
}
