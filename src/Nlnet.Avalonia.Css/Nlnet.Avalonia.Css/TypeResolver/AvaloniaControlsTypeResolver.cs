using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css;

public class AvaloniaControlsTypeResolver : ITypeResolver
{
    private readonly Dictionary<string, Type> _types;

    public AvaloniaControlsTypeResolver()
    {
        var typeSink = typeof(Control);
        var assembly = typeSink.Assembly;
        var types    = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IVisual)));

        _types = types.ToDictionary(type => type.Name, type => type);
    }

    public bool TryGetType(string name, out Type? type)
    {
        return _types.TryGetValue(name, out type);
    }
}
