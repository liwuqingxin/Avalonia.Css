namespace Nlnet.Avalonia.Css;

/// <summary>
/// Use this building css components.
/// </summary>
public interface ICssBuilder : ITypeResolverManager, ICssConfiguration, IResourceProvidersManager
{
    /// <summary>
    /// Build a <see cref="ICssLoader"/> that can be used to load css files.
    /// </summary>
    /// <returns></returns>
    public ICssLoader BuildLoader();
}