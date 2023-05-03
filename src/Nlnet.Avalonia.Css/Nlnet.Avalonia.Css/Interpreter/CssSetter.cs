using System;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class CssSetter
{
    private string RawSetter { get; set; }

    private string? Property { get; set; }

    private string? RawValue { get; set; }

    public CssSetter(string setter)
    {
        RawSetter = setter;
        var splits = setter.Split(":", StringSplitOptions.RemoveEmptyEntries);
        if (splits.Length != 2)
        {
            this.WriteLine($"Invalid setter string : '{setter}'. Skip it.");
            return;
        }
        Property = splits[0];
        RawValue = splits[1];
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

        var value = InterpreterHelper.ParseValue(property, RawValue?.Trim());
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
