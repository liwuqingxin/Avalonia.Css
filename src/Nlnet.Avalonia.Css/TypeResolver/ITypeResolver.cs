using System;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Interface that can provide the service of resolving type from string.
/// </summary>
public interface ITypeResolver
{
    /// <summary>
    /// Try get type from string.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns>True if succeeds to get the type.</returns>
    public bool TryGetType(string name, out Type? type);
}