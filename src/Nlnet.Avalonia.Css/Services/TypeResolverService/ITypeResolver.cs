using System;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Interface that can provide the service of resolving type from string.
/// </summary>
public interface ITypeResolver
{
    /// <summary>
    /// Try to add a name-type pair.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool TryAddType(string name, Type type);

    /// <summary>
    /// Try to add a name-type pair.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool TryAddType<T>(string name);

    /// <summary>
    /// Try to get type from string.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns>True if succeeds to get the type.</returns>
    public bool TryGetType(string name, out Type? type);
}