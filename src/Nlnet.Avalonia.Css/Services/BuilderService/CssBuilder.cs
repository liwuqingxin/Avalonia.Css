using System;
using System.Threading;
using Avalonia.Controls;

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
                throw new InvalidOperationException($"{nameof(CssExtension)}.{nameof(CssExtension.UseAvaloniaCssDefaultBuilder)}() or {nameof(CssBuilder)}.{nameof(CssBuilder.UseDefaultBuilder)}() must be called before accessing the {nameof(CssBuilder)}.{nameof(Default)}.");
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
        Parser          = new CssParser(this);
        Interpreter     = new CssInterpreter(this);
        SectionFactory  = new CssSectionFactory(this);
        ResourceFactory = new CssResourceFactory(this);
    }

    #endregion



    #region ICssBuilder

    private ICssBuilder Internal => (ICssBuilder)this;

    private ICssParser Parser { get; set; }

    private ICssInterpreter Interpreter { get; set; }

    private ICssSectionFactory SectionFactory { get; set; }

    private ICssResourceFactory ResourceFactory { get; set; }



    private ICssLoader? _loader;

    ICssParser ICssBuilder.Parser => Parser;

    ICssInterpreter ICssBuilder.Interpreter => Interpreter;

    ICssSectionFactory ICssBuilder.SectionFactory => SectionFactory;

    ICssResourceFactory ICssBuilder.ResourceFactory => ResourceFactory;

    ITypeResolverManager ICssBuilder.TypeResolver { get; } = new TypeResolverManager();

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


    
    #region ITypeResolverManager

    void ITypeResolverManager.LoadResolver(ITypeResolver resolver)
    {
        Internal.TypeResolver.LoadResolver(resolver);
    }

    void ITypeResolverManager.UnloadResolver(ITypeResolver resolver)
    {
        Internal.TypeResolver.UnloadResolver(resolver);
    }

    void ITypeResolverManager.LoadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        Internal.TypeResolver.LoadValueParsingTypeAdapter(adapter);
    }

    void ITypeResolverManager.UnloadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        Internal.TypeResolver.UnloadValueParsingTypeAdapter(adapter);
    }

    bool ITypeResolverManager.TryGetType(string name, out Type? type)
    {
        return Internal.TypeResolver.TryGetType(name, out type);
    }

    bool ITypeResolverManager.TryAdaptType(Type type, out Type? adaptedType)
    {
        return Internal.TypeResolver.TryAdaptType(type, out adaptedType);
    }

    #endregion


    
    #region ICssConfiguration

    public string? Theme
    {
        get => Internal.Configuration.Theme;
        set => Internal.Configuration.Theme = value;
    }
    public string? Mode
    {
        get => Internal.Configuration.Mode;
        set => Internal.Configuration.Mode = value;
    }

    #endregion



    #region IResourceHostsManager

    private readonly IResourceProvidersManager _resourceProvidersManager = new ResourceProvidersManager();
 

    void IResourceProvidersManager.RegisterResourceProvider(IResourceProvider provider)
    {
        _resourceProvidersManager.RegisterResourceProvider(provider);
    }

    void IResourceProvidersManager.UnregisterResourceProvider(IResourceProvider provider)
    {
        _resourceProvidersManager.UnregisterResourceProvider(provider);
    }

    public bool TryFindResource<T>(object key, out T? result)
    {
        return _resourceProvidersManager.TryFindResource(key, out result);
    }

    #endregion
}
