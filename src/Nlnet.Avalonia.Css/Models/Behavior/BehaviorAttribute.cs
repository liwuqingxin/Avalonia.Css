using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BehaviorAttribute : Attribute
{
    public string Name { get; set; }

    public Type TargetType { get; set; }

    public BehaviorAttribute(string name, Type targetType)
    {
        Name       = name;
        TargetType = targetType;
    }

    public BehaviorAttribute(string name)
    {
        Name       = name;
        TargetType = typeof(StyledElement);
    }
}
