namespace Nlnet.Avalonia.Css;

/// <summary>
/// Use this building css components.
/// </summary>
public interface ICssBuilder : ITypeResolverManager, ICssConfiguration, IResourceProvidersManager
{
    internal ICssParser Parser { get; }

    internal ICssInterpreter Interpreter { get; }

    internal ICssSectionFactory SectionFactory { get; }

    internal ICssResourceFactory ResourceFactory { get; }

    internal ITypeResolverManager TypeResolver { get; }

    /// <summary>
    /// Css configuration.
    /// </summary>
    public ICssConfiguration Configuration { get; }

    /// <summary>
    /// Build a <see cref="ICssLoader"/> that can be used to load css files.
    /// </summary>
    /// <returns></returns>
    public ICssLoader BuildLoader();
}