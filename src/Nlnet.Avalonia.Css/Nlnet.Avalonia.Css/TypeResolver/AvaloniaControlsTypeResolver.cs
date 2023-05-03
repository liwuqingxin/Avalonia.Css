using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

internal class AvaloniaControlsTypeResolver : ITypeResolver
{
    private readonly Dictionary<string, Type> _types;

    public AvaloniaControlsTypeResolver()
    {
        var typeSink = typeof(Control);
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