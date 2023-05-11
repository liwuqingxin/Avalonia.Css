using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class PropertySyntax : ISyntax
{
    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        if (previous?.TargetType != null)
        {
            var avaloniaProperty = InterpreterHelper.GetAvaloniaProperty(previous.TargetType, Property);
            if (avaloniaProperty == null)
            {
                return previous;
            }
            var value = InterpreterHelper.ParseValue(avaloniaProperty, Value);
            if (value != null)
            {
                return previous.PropertyEquals(avaloniaProperty, value);
            }
        }

        this.WriteLine($"Previous selector's TargetType is null. '[{Property}={Value}]' skipped.");

        return previous;
    }
}
