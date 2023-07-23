using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

internal class TypeResolverManager : ITypeResolverManager
{
    private readonly List<ITypeResolver> _resolvers = new()
    {
        // Avalonia.Controls
        new GenericResolver<Control>(),
        // Avalonia.Base
        new GenericResolver<Transform>(),
        // Internal Resolver.
        new InternalResolver(),
    };

    private readonly List<IValueParsingTypeAdapter> _typeAdapters = new()
    {
        // Internal value parsing type adapter.
        new InternalValueParsingTypeAdapter(),
    };

    public void LoadResolver(ITypeResolver resolver)
    {
        _resolvers.Add(resolver);
    }

    public void UnloadResolver(ITypeResolver resolver)
    {
        _resolvers.Remove(resolver);
    }

    public void LoadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        _typeAdapters.Add(adapter);
    }

    public void UnloadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        _typeAdapters.Remove(adapter);
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

        this.WriteLine($"Can not resolve type '{name}'.");

        type = null;
        return false;
    }

    public bool TryAdaptType(Type type, out Type? adaptedType)
    {
        foreach (var adapter in _typeAdapters)
        {
            if (adapter.TryAdapt(type, out adaptedType))
            {
                return true;
            }
        }

        this.WriteLine($"Can not adapt type '{type}'.");

        adaptedType = null;
        return false;
    }
}
