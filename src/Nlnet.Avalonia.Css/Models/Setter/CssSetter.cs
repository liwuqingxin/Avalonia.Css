using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface ICssSetter
{
    public string? Property { get; set; }

    public string? RawValue { get; set; }

    public Setter? ToAvaloniaSetter(ICssBuilder builder, Type targetType);
}

internal class CssSetter : ICssSetter
{
    public string? Property { get; set; }

    public string? RawValue { get; set; }

    public CssSetter(string setterString)
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

    public CssSetter(string name, string value)
    {
        Property = name;
        RawValue = value;
    }

    public Setter? ToAvaloniaSetter(ICssBuilder builder, Type targetType)
    {
        if (Property == null)
        {
            return null;
        }

        var interpreter = builder.Interpreter;
        var property =
            interpreter.ParseAvaloniaProperty(targetType, Property)
            ?? interpreter.ParseAcssBehaviorProperty(targetType, Property, RawValue);

        if (property == null)
        {
            return null;
        }

        var value = interpreter.ParseValue(property, RawValue?.Trim());
        if (value == null)
        {
            if (property.PropertyType.IsValueType)
            {
                value = Activator.CreateInstance(property.PropertyType);
            }
        }

#pragma warning disable CS8604
        return new Setter(property, value);
#pragma warning restore CS8604
    }

    public override string ToString()
    {
        return $"{Property}:{RawValue}";
    }
}
