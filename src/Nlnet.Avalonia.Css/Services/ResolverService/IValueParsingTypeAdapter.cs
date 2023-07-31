using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Interface that can provide the service to adapt the source type to the type that has the 'Parse' method for the source type.
/// </summary>
public interface IValueParsingTypeAdapter
{
    /// <summary>
    /// Adapt the source type to the type that has the 'Parse' method.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="adaptedType"></param>
    /// <returns></returns>
    public bool TryAdapt(Type type, out Type? adaptedType);
}

/// <summary>
/// A generic implementation for IValueParsingTypeAdapter.
/// </summary>
public abstract class ValueParsingTypeAdapter : IValueParsingTypeAdapter
{
    private readonly Dictionary<Type, Type> _parseTypeAdapter = new();

    public bool TryAdapt(Type type, out Type? adaptedType)
    {
        return _parseTypeAdapter.TryGetValue(type, out adaptedType);
    }

    protected void AddAdaptType(Type declaredType, Type parsingType)
    {
        _parseTypeAdapter[declaredType] = parsingType;

        this.WriteLine($"Add value parsing adapt type '{parsingType}' for '{declaredType}'");
    }
}