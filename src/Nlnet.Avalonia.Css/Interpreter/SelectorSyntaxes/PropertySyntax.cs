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
            var avaloniaProperty = ServiceLocator.GetService<ICssInterpreter>().ParseAvaloniaProperty(previous.TargetType, Property);
            if (avaloniaProperty == null)
            {
                return previous;
            }
            var value = ServiceLocator.GetService<ICssInterpreter>().ParseValue(avaloniaProperty, Value);
            if (value != null)
            {
                return previous.PropertyEquals(avaloniaProperty, value);
            }
        }

        this.WriteLine($"Previous selector's TargetType is null. '[{Property}={Value}]' skipped.");

        return previous;
    }
}
