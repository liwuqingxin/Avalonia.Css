namespace Nlnet.Avalonia.Css;

/// <summary>
/// Use this building css components.
/// </summary>
public interface ICssBuilder : ITypeResolverManager, IResourceProvidersManager
{
    internal ICssParser Parser { get; }

    internal ICssInterpreter Interpreter { get; }

    internal ICssSectionFactory SectionFactory { get; }

    internal ICssResourceFactory ResourceFactory { get; }

    internal ITypeResolverManager TypeResolver { get; }

    internal bool TryAddCssFile(ICssFile file);

    internal bool TryGetCssFile(string standardFilePath, out ICssFile? file);

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