namespace Nlnet.Avalonia.Css;

/// <summary>
/// Use this building acss components.
/// </summary>
public interface IAcssBuilder
{
    internal IAcssParser Parser { get; }

    internal IAcssInterpreter Interpreter { get; }

    internal IAcssSectionFactory SectionFactory { get; }

    internal IAcssResourceFactory ResourceFactory { get; }

    public ITypeResolverManager TypeResolver { get; }

    public IValueParsingTypeAdapterManager ValueParsingTypeAdapter { get; }

    public IResourceProvidersManager ResourceProvidersManager { get; }

    public IBehaviorDeclarerManager BehaviorDeclarerManager { get; }

    public IBehaviorResolverManager BehaviorResolverManager { get; }

    internal bool TryAddAcssFile(IAcssFile file);

    internal bool TryRemoveAcssFile(IAcssFile file);

    internal bool TryGetAcssFile(string standardFilePath, out IAcssFile? file);



    /// <summary>
    /// Acss configuration.
    /// </summary>
    public IAcssConfiguration Configuration { get; }

    /// <summary>
    /// Build a <see cref="IAcssLoader"/> that can be used to load acss files.
    /// </summary>
    /// <returns></returns>
    public IAcssLoader BuildLoader();

    /// <summary>
    /// Build a rider settings file to support acss language according to this builder instance.
    /// Now we do not provide language support in rider. Use this instead now.
    /// </summary>
    public void BuildRiderSettingsForAcss();
}