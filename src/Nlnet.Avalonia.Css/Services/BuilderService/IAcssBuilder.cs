using System;

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
    /// Build a rider settings file to support acss language according to this builder instance. <br/><br/>
    /// Now we do not provide language supporting plugin in rider. Use this simple rider setting instead now. <br/><br/>
    /// This function will try to create a setting file in rider's folder which is like
    /// "C:\Users\72975\AppData\Roaming\JetBrains\Rider2023.1\filetypes\Acss.xml". <br/><br/>
    /// If failed, you could handle it with the <see cref="output"/>, <see cref="setting"/> and <see cref="exceptionHandler"/>.
    /// </summary>
    /// <param name="output">The out put file path if succeed.</param>
    /// <param name="setting">The setting file xml content.</param>
    /// <param name="exceptionHandler">Exception handler that could be null.</param>
    /// <returns></returns>
    public bool TryBuildRiderSettingsForAcss(out string? output, out string? setting, Action<Exception>? exceptionHandler = null);
}