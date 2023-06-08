using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace Nlnet.Avalonia.Css;

public class GenericResolver<T> : ITypeResolver
{
    private readonly Dictionary<string, Type> _types;

    public GenericResolver()
    {
        var typeSink = typeof(T);
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