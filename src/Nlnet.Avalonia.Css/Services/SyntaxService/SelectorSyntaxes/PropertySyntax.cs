using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class PropertySyntax : ISyntax
{
    public string Property { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, Selector? previous)
    {
        var previousTargetType = previous?.GetTargetType() ?? acssStyle.GetTargetType();
        if (previousTargetType == null)
        {
            context.OnError(AcssErrors.Style_TargetType_Not_Found, 
                $"Previous selector's TargetType and parent target type is null. '[{Property}={Value}]' skipped.");
            return previous;
        }

        var interpreter = context.GetService<IAcssInterpreter>();

        var avaloniaProperty = interpreter.ParseAvaloniaProperty(previousTargetType, Property);
        if (avaloniaProperty == null)
        {
            context.OnError(AcssErrors.Property_Not_Found, 
                $"Can not resolve the property '{Property}' of the type '{previousTargetType}'. '[{Property}={Value}]' skipped.");
            return previous;
        }
        var value = interpreter.ParseValue(avaloniaProperty, Value);
        if (value == null)
        {
            context.OnError(AcssErrors.Value_String_Invalid, 
                $"Can not resolve the value '{Value}' for property '{Property}'. Skip it.");
            return previous;
        }

        return previous.PropertyEquals(avaloniaProperty, value);
    }
}
