using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Internal type resolver.
/// </summary>
internal class InternalResolver : ITypeResolver
{
    private readonly Dictionary<string, Type> _types = new();

    public InternalResolver()
    {
        _types.Add(nameof(TextDecorations), typeof(TextDecorations));
        //_types.Add(nameof(RhythmicTransformAnimator), typeof(RhythmicTransformAnimator));
    }

    public bool TryAddType(string name, Type type)
    {
        if (_types.ContainsKey(name))
        {
            return false;
        }

        _types.Add(name, type);

        return true;
    }

    public bool TryAddType<T>(string name)
    {
        if (_types.ContainsKey(name))
        {
            return false;
        }

        _types.Add(name, typeof(T));

        return true;
    }

    public bool TryGetType(string name, out Type? type)
    {
        return _types.TryGetValue(name, out type);
    }
}
