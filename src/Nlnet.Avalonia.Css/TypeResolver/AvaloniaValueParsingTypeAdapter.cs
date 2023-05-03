﻿using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

internal class AvaloniaValueParsingTypeAdapter : IValueParsingTypeAdapter
{
    private readonly Dictionary<Type, Type> _parseTypeAdapter = new();

    public AvaloniaValueParsingTypeAdapter()
    {
        AddAdaptType(typeof(IBrush),           typeof(Brush));
        AddAdaptType(typeof(ISolidColorBrush), typeof(Brush));
        AddAdaptType(typeof(SolidColorBrush),  typeof(Brush));
    }

    public bool TryAdapt(Type type, out Type? adaptedType)
    {
        return _parseTypeAdapter.TryGetValue(type, out adaptedType);
    }

    public void AddAdaptType(Type declaredType, Type parsingType)
    {
        _parseTypeAdapter[declaredType] = parsingType;

        this.WriteLine($"Add value parsing adapt type '{declaredType}' to '{parsingType}'");
    }
}