using System;
using System.Collections.Generic;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Transformation;

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

        this.WriteLine($"Add value parsing adapt type '{declaredType}' to '{parsingType}'");
    }
}

internal class AvaloniaDefaultValueParsingTypeAdapter : GenericValueParsingTypeAdapter
{
    public AvaloniaDefaultValueParsingTypeAdapter()
    {
        AddAdaptType(typeof(IBrush),           typeof(Brush));
        AddAdaptType(typeof(ISolidColorBrush), typeof(Brush));
        AddAdaptType(typeof(SolidColorBrush),  typeof(Brush));
        AddAdaptType(typeof(ITransform),       typeof(TransformOperations));
        AddAdaptType(typeof(Transform),        typeof(TransformOperations));
        AddAdaptType(typeof(Transitions),      typeof(TransitionsParser));
        AddAdaptType(typeof(KeyFrames),        typeof(KeyFramesParser));
    }
}
