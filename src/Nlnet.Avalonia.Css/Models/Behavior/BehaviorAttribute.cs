using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Mark up the class is an acss behavior implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BehaviorAttribute : Attribute
{
    /// <summary>
    /// The name of the acss behavior that acss syntax can recognized.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The type acss behavior belongs to.
    /// </summary>
    public Type OwnerType { get; set; }

    public BehaviorAttribute(string name, Type ownerType)
    {
        Name      = name;
        OwnerType = ownerType;
    }
}
