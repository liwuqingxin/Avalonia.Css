using System;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public interface ICssSetter
{
    public string Property { get; set; }

    public string? RawValue { get; set; }

    public ISetter? ToAvaloniaSetter(Type targetType);
}

public class CssSetter : ICssSetter
{
    public string? Property { get; set; }

    public string? RawValue { get; set; }

    public CssSetter(string setterString)
    {
        var splits = setterString.Split(":", StringSplitOptions.RemoveEmptyEntries);
        if (splits.Length != 2)
        {
            this.WriteLine($"Invalid setter string : '{setterString}'. Skip it.");
            return;
        }
        Property = splits[0];
        RawValue = splits[1];
    }

    public CssSetter(string name, string value)
    {
        Property = name;
        RawValue = value;
    }

    public ISetter? ToAvaloniaSetter(Type targetType)
    {
        if (Property == null)
        {
            return null;
        }

        var interpreter = ServiceLocator.GetService<ICssInterpreter>();

        var property = interpreter.ParseAvaloniaProperty(targetType, Property);
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
