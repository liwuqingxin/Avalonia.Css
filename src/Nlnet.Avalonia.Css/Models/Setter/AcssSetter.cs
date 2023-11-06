using System;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface IAcssSetter
{
    public string? Property { get; set; }

    public string? RawValue { get; set; }

    public Setter? ToAvaloniaSetter(IAcssContext context, Type targetType);
}

internal class AcssSetter : IAcssSetter
{
    public string? Property { get; set; }

    public string? RawValue { get; set; }

    public AcssSetter(string setterString)
    {
        var index = setterString.IndexOf(':');
        if (index == -1)
        {
            this.WriteError($"Invalid setter string : '{setterString}'. Skip it.");
            return;
        }
        Property = setterString[..index];
        RawValue = setterString.Substring(index + 1, setterString.Length - index - 1);
    }

    public AcssSetter(string name, string value)
    {
        Property = name;
        RawValue = value;
    }

    public Setter? ToAvaloniaSetter(IAcssContext context, Type targetType)
    {
        if (Property == null)
        {
            return null;
        }

        var interpreter = context.GetService<IAcssInterpreter>();
        var property = interpreter.ParseAvaloniaProperty(targetType, Property);
        object? value;
        if (property == null)
        {
            property = interpreter.ParseAcssBehaviorProperty(targetType, Property, RawValue, out var behavior);
            if (property == null)
            {
                return null;
            }

            value = new BehaviorTemplate(() => behavior?.Get());
        }
        else
        {
            value = interpreter.ParseDynamicValue(property, RawValue?.Trim());
            if (value == null)
            {
                if (property.PropertyType.IsValueType)
                {
                    value = Activator.CreateInstance(property.PropertyType);
                }
            }
        }

        return new Setter(property, value);
    }

    public override string ToString()
    {
        return $"{Property}:{RawValue}";
    }
}
