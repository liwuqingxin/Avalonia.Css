using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// For resolving management.
/// </summary>
public interface IResolverManager<in T> where T : IResolver
{
    /// <summary>
    /// Load a <see cref="T"/>.
    /// </summary>
    /// <param name="resolver"></param>
    public void LoadResolver(T resolver);

    /// <summary>
    /// Unload a <see cref="T"/>.
    /// </summary>
    /// <param name="resolver"></param>
    public void UnloadResolver(T resolver);

    /// <summary>
    /// Try get type with the name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool TryGetType(string name, out Type? type);

    /// <summary>
    /// Get all types.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Type> GetAllTypes();
}

/// <summary>
/// A base implementation for <see cref="IResolverManager{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ResolverManager<T> : IResolverManager<T> where T : IResolver
{
    private readonly List<T> _resolvers = new();

    public void LoadResolver(T resolver)
    {
        _resolvers.Add(resolver);
    }

    public void UnloadResolver(T resolver)
    {
        _resolvers.Remove(resolver);
    }

    public bool TryGetType(string name, out Type? type)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.TryGetType(name, out type))
            {
                return true;
            }
        }

        this.WriteError($"Can not resolve type '{name}'.");

        type = null;
        return false;
    }

    public IEnumerable<Type> GetAllTypes()
    {
        return _resolvers.SelectMany(r => r.GetAllTypes());
    }
}
