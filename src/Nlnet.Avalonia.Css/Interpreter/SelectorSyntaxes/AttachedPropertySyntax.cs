using Avalonia.Styling;
using System;

namespace Nlnet.Avalonia.Css;

public class AttachedPropertySyntax : ISyntax, ITypeSyntax
{
    public string Xmlns { get; set; } = string.Empty;

    public string TypeName { get; set; } = string.Empty;

    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(Selector? previous)
    {
        var manager = CssServiceLocator.GetService<ITypeResolverManager>();
        if (manager.TryGetType(TypeName, out var type))
        {
            var interpreter      = CssServiceLocator.GetService<ICssInterpreter>();
            var avaloniaProperty = interpreter.ParseAvaloniaProperty(type!, Property);
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

        return previous;
    }
}
