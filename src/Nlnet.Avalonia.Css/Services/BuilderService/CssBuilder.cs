using System;
using System.Collections.Concurrent;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class CssBuilder : ICssBuilder
{
    #region Default ICssBuilder

    private static int          _prepared;
    private static ICssBuilder? _default;

    public static ICssBuilder Default
    {
        get
        {
            if (_default == null)
            {
                throw new InvalidOperationException($"{nameof(CssExtension)}.{nameof(CssExtension.UseAcssDefaultBuilder)}() or {nameof(CssBuilder)}.{nameof(CssBuilder.UseDefaultBuilder)}() must be called before accessing the {nameof(CssBuilder)}.{nameof(Default)}.");
            }
            return _default;
        }
    }

    /// <summary>
    /// Use default <see cref="ICssBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static ICssBuilder UseDefaultBuilder()
    {
        if (Interlocked.Exchange(ref _prepared, 1) == 0)
        {
            _default = new CssBuilder();
        }

        return _default!;
    }

    public CssBuilder()
    {
        _parser          = new CssParser(this);
        _interpreter     = new CssInterpreter(this);
        _sectionFactory  = new CssSectionFactory(this);
        _resourceFactory = new CssResourceFactory(this);

        Internal.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss).ToLower());
        Internal.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss));
    }

    #endregion



    #region ICssBuilder

    private ICssBuilder Internal => (ICssBuilder)this;

    private readonly ICssParser _parser;
    private readonly ICssInterpreter _interpreter;
    private readonly ICssSectionFactory _sectionFactory;
    private readonly ICssResourceFactory _resourceFactory;



    private ICssLoader? _loader;
    private readonly ConcurrentDictionary<string, ICssFile> _files = new();

    ICssParser ICssBuilder.Parser => _parser;

    ICssInterpreter ICssBuilder.Interpreter => _interpreter;

    ICssSectionFactory ICssBuilder.SectionFactory => _sectionFactory;

    ICssResourceFactory ICssBuilder.ResourceFactory => _resourceFactory;

    ITypeResolverManager ICssBuilder.TypeResolver { get; } = new TypeResolverManager();

    IValueParsingTypeAdapterManager ICssBuilder.ValueParsingTypeAdapter { get; } = new ValueParsingTypeAdapterManager();

    IResourceProvidersManager ICssBuilder.ResourceProvidersManager { get; } = new ResourceProvidersManager();

    IBehaviorDeclarerManager ICssBuilder.BehaviorDeclarerManager { get; } = new BehaviorDeclarerManager();

    IBehaviorResolverManager ICssBuilder.BehaviorResolverManager { get; } = new BehaviorResolverManager();

    bool ICssBuilder.TryAddCssFile(ICssFile file)
    {
        if (_files.TryGetValue(file.StandardFilePath, out _))
        {
            return false;
        }

        _files.TryAdd(file.StandardFilePath, file);
        return true;
    }

    bool ICssBuilder.TryRemoveCssFile(ICssFile file)
    {
        return _files.TryRemove(file.StandardFilePath, out _);
    }

    bool ICssBuilder.TryGetCssFile(string standardFilePath, out ICssFile? file)
    {
        if (_files.TryGetValue(standardFilePath, out var f))
        {
            file = f;
            return true;
        }

        file = null;
        return false;
    }

    ICssConfiguration ICssBuilder.Configuration { get; } = new CssConfiguration();

    ICssLoader ICssBuilder.BuildLoader()
    {
        lock (this)
        {
            _loader ??= new CssLoader(this);    
        }
        
        return _loader;
    }

    #endregion
}
