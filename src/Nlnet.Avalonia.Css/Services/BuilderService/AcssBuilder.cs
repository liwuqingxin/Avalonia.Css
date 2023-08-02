using System;
using System.Collections.Concurrent;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class AcssBuilder : IAcssBuilder
{
    #region Default ICssBuilder

    private static int           _prepared;
    private static IAcssBuilder? _default;

    public static IAcssBuilder Default
    {
        get
        {
            if (_default == null)
            {
                throw new InvalidOperationException($"{nameof(AcssExtension)}.{nameof(AcssExtension.UseAcssDefaultBuilder)}() or {nameof(AcssBuilder)}.{nameof(AcssBuilder.UseDefaultBuilder)}() must be called before accessing the {nameof(AcssBuilder)}.{nameof(Default)}.");
            }
            return _default;
        }
    }

    /// <summary>
    /// Use default <see cref="IAcssBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static IAcssBuilder UseDefaultBuilder()
    {
        if (Interlocked.Exchange(ref _prepared, 1) == 0)
        {
            _default = new AcssBuilder();
        }

        return _default!;
    }

    public AcssBuilder()
    {
        _parser          = new AcssParser(this);
        _interpreter     = new AcssInterpreter(this);
        _sectionFactory  = new AcssSectionFactory(this);
        _resourceFactory = new AcssResourceFactory(this);
    }

    #endregion



    #region ICssBuilder

    private IAcssBuilder Internal => (IAcssBuilder)this;

    private readonly IAcssParser _parser;
    private readonly IAcssInterpreter _interpreter;
    private readonly IAcssSectionFactory _sectionFactory;
    private readonly IAcssResourceFactory _resourceFactory;



    private IAcssLoader? _loader;
    private readonly ConcurrentDictionary<string, IAcssFile> _files = new();

    IAcssParser IAcssBuilder.Parser => _parser;

    IAcssInterpreter IAcssBuilder.Interpreter => _interpreter;

    IAcssSectionFactory IAcssBuilder.SectionFactory => _sectionFactory;

    IAcssResourceFactory IAcssBuilder.ResourceFactory => _resourceFactory;

    ITypeResolverManager IAcssBuilder.TypeResolver { get; } = new TypeResolverManager();

    IValueParsingTypeAdapterManager IAcssBuilder.ValueParsingTypeAdapter { get; } = new ValueParsingTypeAdapterManager();

    IResourceProvidersManager IAcssBuilder.ResourceProvidersManager { get; } = new ResourceProvidersManager();

    IBehaviorDeclarerManager IAcssBuilder.BehaviorDeclarerManager { get; } = new BehaviorDeclarerManager();

    IBehaviorResolverManager IAcssBuilder.BehaviorResolverManager { get; } = new BehaviorResolverManager();

    bool IAcssBuilder.TryAddAcssFile(IAcssFile file)
    {
        if (_files.TryGetValue(file.StandardFilePath, out _))
        {
            return false;
        }

        _files.TryAdd(file.StandardFilePath, file);
        return true;
    }

    bool IAcssBuilder.TryRemoveAcssFile(IAcssFile file)
    {
        return _files.TryRemove(file.StandardFilePath, out _);
    }

    bool IAcssBuilder.TryGetAcssFile(string standardFilePath, out IAcssFile? file)
    {
        if (_files.TryGetValue(standardFilePath, out var f))
        {
            file = f;
            return true;
        }

        file = null;
        return false;
    }

    IAcssConfiguration IAcssBuilder.Configuration { get; } = new AcssConfiguration();

    IAcssLoader IAcssBuilder.BuildLoader()
    {
        lock (this)
        {
            _loader ??= new AcssLoader(this);    
        }
        
        return _loader;
    }

    #endregion
}
