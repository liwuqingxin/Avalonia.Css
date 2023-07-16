using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class PropertySyntax : ISyntax
{
    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(ICssBuilder builder, Selector? previous)
    {
        var previousTargetType = previous?.GetTargetType();
        if (previousTargetType != null)
        {
            var interpreter = builder.Interpreter;

            var avaloniaProperty = interpreter.ParseAvaloniaProperty(previousTargetType, Property);
            if (avaloniaProperty == null)
            {
                return previous;
            }
            var value = interpreter.ParseValue(avaloniaProperty, Value);
            if (value != null)
            {
                return previous.PropertyEquals(avaloniaProperty, value);
            }
        }

        this.WriteLine($"Previous selector's TargetType is null. '[{Property}={Value}]' skipped.");

        return previous;
    }
}
