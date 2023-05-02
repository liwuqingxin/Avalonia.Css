using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

public class TypeResolverManager
{
    public static TypeResolverManager Instance { get; } = new();

    private readonly Dictionary<Type, Type> _parseTypeAdapter = new();

    private TypeResolverManager()
    {
        AddParseType(typeof(IBrush),           typeof(Brush));
        AddParseType(typeof(ISolidColorBrush), typeof(Brush));
        AddParseType(typeof(SolidColorBrush),  typeof(Brush));
    }

    private readonly List<ITypeResolver> _resolvers = new()
    {
        new AvaloniaControlsTypeResolver(),
    };

    public void LoadResolver(ITypeResolver resolver)
    {
        _resolvers.Add(resolver);
    }

    public void UnloadResolver(ITypeResolver resolver)
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

        type = null;
        return false;
    }

    public void AddParseType(Type declaredType, Type parseType)
    {
        _parseTypeAdapter[declaredType] = parseType;
    }

    public bool TryGetParseType(Type declaredType, out Type? parseType)
    {
        return _parseTypeAdapter.TryGetValue(declaredType, out parseType);
    }
}
