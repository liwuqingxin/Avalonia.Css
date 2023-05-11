using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class AttachedPropertySyntax : ISyntax, ITypeSyntax
{
    public string Xmlns { get; set; } = string.Empty;

    public string TypeName { get; set; } = string.Empty;

    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        if (TypeResolver.Instance.TryGetType(TypeName, out var type))
        {
            var avaloniaProperty = InterpreterHelper.ParseAvaloniaProperty(type!, Property);
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

        return previous;
    }
}
