using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class PropertySyntax : ISyntax
{
    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        if (previous?.TargetType != null)
        {
            var interpreter = ServiceLocator.GetService<ICssInterpreter>();

            var avaloniaProperty = interpreter.ParseAvaloniaProperty(previous.TargetType, Property);
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
