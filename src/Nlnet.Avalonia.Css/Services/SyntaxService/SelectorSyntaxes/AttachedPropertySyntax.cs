using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class AttachedPropertySyntax : ISyntax, ITypeSyntax
{
    public string Xmlns { get; set; } = string.Empty;

    public string TypeName { get; set; } = string.Empty;

    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        var manager = context.GetService<ITypeResolverManager>();
        var interpreter = context.GetService<IAcssInterpreter>();

        if (manager.TryGetType(TypeName, out var type))
        {
            var avaloniaProperty = interpreter.ParseAvaloniaProperty(type!, Property);
            if (avaloniaProperty == null)
            {
                context.OnError(AcssErrors.Property_Not_Found, 
                    $"Can not resolve the property '{Property}' of the type '{type}'. '[{Property}={Value}]' skipped.");
                return previous;
            }
            var value = interpreter.ParseValue(avaloniaProperty, Value);
            if (value == null)
            {
                context.OnError(AcssErrors.Value_String_Invalid, 
                    $"Can not resolve the value '{Value}' for property '{type}.{Property}'. Skip it.");
                return previous;
            }
            return previous.PropertyEquals(avaloniaProperty, value);
        }

        context.OnError(AcssErrors.Type_Not_Found, 
            $"Can not resolve the type '{TypeName}'. '[{Property}={Value}]' skipped.");
        return previous;
    }
}
