using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A generic implementation for <see cref="ITypeResolver"/> that can provide type resolving service for assembly who contains the class of <see cref="TTypeSink"/>.
/// </summary>
/// <typeparam name="TTypeSink"></typeparam>
public class GenericResolver<TTypeSink> : ITypeResolver
{
    private readonly Dictionary<string, Type> _types;

    public GenericResolver()
    {
        var typeSink = typeof(TTypeSink);
        var assembly = typeSink.Assembly;
        var types    = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(AvaloniaObject)));

        _types = types.ToDictionary(type => type.Name, type => type);

        foreach (var type in _types)
        {
            this.WriteLine($"Map '{type.Key}' to '{type.Value}'");
        }
    }

    public bool TryGetType(string name, out Type? type)
    {
        return _types.TryGetValue(name, out type);
    }
}