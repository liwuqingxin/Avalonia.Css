using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

public interface ITypeResolverManager
{
    public void LoadResolver(ITypeResolver resolver);

    public void UnloadResolver(ITypeResolver resolver);

    public void LoadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter);

    public void UnloadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter);

    public bool TryGetType(string name, out Type? type);

    public bool TryAdaptType(Type type, out Type? adaptedType);
}

public class TypeResolverManager : ITypeResolverManager
{
    private readonly List<ITypeResolver> _resolvers = new()
    {
        new GenericResolver<Control>(),
    };

    private readonly List<IValueParsingTypeAdapter> _typeAdapters = new()
    {
        new AvaloniaDefaultValueParsingTypeAdapter(),
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
