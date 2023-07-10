using System;

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
