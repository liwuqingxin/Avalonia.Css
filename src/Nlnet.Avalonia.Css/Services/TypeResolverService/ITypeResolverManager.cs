using System;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// For type resolving.
/// </summary>
public interface ITypeResolverManager
{
    /// <summary>
    /// Load a <see cref="ITypeResolver"/>.
    /// </summary>
    /// <param name="resolver"></param>
    public void LoadResolver(ITypeResolver resolver);

    /// <summary>
    /// Unload a <see cref="ITypeResolver"/>.
    /// </summary>
    /// <param name="resolver"></param>
    public void UnloadResolver(ITypeResolver resolver);

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
    /// Try get type with the name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool TryGetType(string name, out Type? type);

    /// <summary>
    /// Try get the adapted parsing type for the type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="adaptedType"></param>
    /// <returns></returns>
    public bool TryAdaptType(Type type, out Type? adaptedType);
}
