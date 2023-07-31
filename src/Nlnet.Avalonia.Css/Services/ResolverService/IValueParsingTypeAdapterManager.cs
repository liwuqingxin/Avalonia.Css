using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// For type resolving.
/// </summary>
public interface IValueParsingTypeAdapterManager
{
    /// <summary>
    /// Load a <see cref="IValueParsingTypeAdapter"/>.
    /// </summary>
    /// <param name="adapter"></param>
    public void LoadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter);

    /// <summary>
    /// Unload a <see cref="IValueParsingTypeAdapter"/>.
    /// </summary>
    /// <param name="adapter"></param>
    public void UnloadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter);
    
    /// <summary>
    /// Try get the adapted parsing type for the type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="adaptedType"></param>
    /// <returns></returns>
    public bool TryAdaptType(Type type, out Type? adaptedType);
}

internal class ValueParsingTypeAdapterManager : IValueParsingTypeAdapterManager
{
    private readonly List<IValueParsingTypeAdapter> _typeAdapters = new()
    {
        // Internal value parsing type adapter.
        new InternalValueParsingTypeAdapter(),
    };

    public void LoadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        _typeAdapters.Add(adapter);
    }

    public void UnloadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        _typeAdapters.Remove(adapter);
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

        //this.WriteWarning($"Can not adapt type '{type}'.");

        adaptedType = null;
        return false;
    }
}