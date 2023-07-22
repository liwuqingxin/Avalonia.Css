using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A generic implementation for IValueParsingTypeAdapter.
/// </summary>
public abstract class GenericValueParsingTypeAdapter : IValueParsingTypeAdapter
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