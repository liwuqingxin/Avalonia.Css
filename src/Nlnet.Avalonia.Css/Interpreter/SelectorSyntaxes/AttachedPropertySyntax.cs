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
        var manager = ServiceLocator.GetService<ITypeResolverManager>();
        if (manager.TryGetType(TypeName, out var type))
        {
            var avaloniaProperty = ServiceLocator.GetService<ICssInterpreter>().ParseAvaloniaProperty(type!, Property);
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

        return previous;
    }
}
