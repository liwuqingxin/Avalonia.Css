using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A common resolver.
/// </summary>
public interface IResolver
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

/// <summary>
/// A base implementation for <see cref="IResolver"/>.
/// </summary>
public class Resolver : IResolver
{
    protected Dictionary<string, Type> Types;

    public Resolver()
    {
        Types = new Dictionary<string, Type>();
    }

    public bool TryAddType(string name, Type type)
    {
        if (Types.ContainsKey(name))
        {
            return false;
        }

        Types.Add(name, type);

        return true;
    }

    public bool TryAddType<T>(string name)
    {
        if (Types.ContainsKey(name))
        {
            return false;
        }

        Types.Add(name, typeof(T));

        return true;
    }

    public bool TryGetType(string name, out Type? type)
    {
        return Types.TryGetValue(name, out type);
    }
}

/// <summary>
/// A generic implementation for <see cref="IResolver"/> that can provide common resolving service for assembly who contains the class of <see cref="TTypeSink"/>.
/// </summary>
/// <typeparam name="TTypeSink"></typeparam>
public class Resolver<TTypeSink> : Resolver
{
    public Resolver()
    {
        var typeSink = typeof(TTypeSink);
        var assembly = typeSink.Assembly;
        var types = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(AvaloniaObject)));

        Types = types.ToDictionary(type => type.Name, type => type);

        foreach (var type in Types)
        {
            this.WriteLine($"Map '{type.Key}' to '{type.Value}'");
        }
    }
}