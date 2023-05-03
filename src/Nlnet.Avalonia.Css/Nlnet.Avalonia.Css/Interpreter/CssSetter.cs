using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class CssSetter
{
    public string RawSetter { get; set; }

    public string? Property { get; set; }

    public string? RawValue { get; set; }

    public CssSetter(string setter)
    {
        RawSetter = setter;
        var splits = setter.Split(":", StringSplitOptions.RemoveEmptyEntries);
        if (splits.Length == 2)
        {
            Property = splits[0];
            RawValue = splits[1];
        }
    }

    public ISetter? ToAvaloniaSetter(Type targetType)
    {
        if (Property == null)
        {
            return null;
        }

        var property = InterpreterHelper.GetAvaloniaProperty(targetType, Property);
        if (property == null)
        {
            return null;
        }

        object? value = RawValue;
        if (property.PropertyType != typeof(string))
        {
            value = InterpreterHelper.ParseValue(property, RawValue?.Trim());
        }

        if (value == null)
        {
            return null;
        }

        return new Setter(property, value);
    }

    public override string ToString()
    {
        return RawSetter;
    }
}
