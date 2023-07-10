using System;
using System.Threading;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

public class CssBuilder : ICssBuilder
{
    private static int          _prepared = 0;
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



    #region ICssBuilder

    private ICssLoader? _loader;

    ICssLoader ICssBuilder.BuildLoader()
    {
        lock (this)
        {
            _loader ??= new CssLoader();    
        }
        
        return _loader;
    }

    #endregion


    
    #region ITypeResolverManager

    void ITypeResolverManager.LoadResolver(ITypeResolver resolver)
    {
        ServiceLocator.GetService<ITypeResolverManager>().LoadResolver(resolver);
    }

    void ITypeResolverManager.UnloadResolver(ITypeResolver resolver)
    {
        ServiceLocator.GetService<ITypeResolverManager>().UnloadResolver(resolver);
    }

    void ITypeResolverManager.LoadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        ServiceLocator.GetService<ITypeResolverManager>().LoadValueParsingTypeAdapter(adapter);
    }

    void ITypeResolverManager.UnloadValueParsingTypeAdapter(IValueParsingTypeAdapter adapter)
    {
        ServiceLocator.GetService<ITypeResolverManager>().UnloadValueParsingTypeAdapter(adapter);
    }

    bool ITypeResolverManager.TryGetType(string name, out Type? type)
    {
        return ServiceLocator.GetService<ITypeResolverManager>().TryGetType(name, out type);
    }

    bool ITypeResolverManager.TryAdaptType(Type type, out Type? adaptedType)
    {
        return ServiceLocator.GetService<ITypeResolverManager>().TryAdaptType(type, out adaptedType);
    }

    #endregion


    
    #region ICssConfiguration

    public string? Theme
    {
        get => ServiceLocator.GetService<ICssConfiguration>().Theme;
        set => ServiceLocator.GetService<ICssConfiguration>().Theme = value;
    }
    public string? Mode
    {
        get => ServiceLocator.GetService<ICssConfiguration>().Mode;
        set => ServiceLocator.GetService<ICssConfiguration>().Mode = value;
    }

    #endregion



    #region IResourceHostsManager

    void IResourceProvidersManager.RegisterResourceProvider(IResourceProvider provider)
    {
        ServiceLocator.GetService<IResourceProvidersManager>().RegisterResourceProvider(provider);
    }

    void IResourceProvidersManager.UnregisterResourceProvider(IResourceProvider provider)
    {
        ServiceLocator.GetService<IResourceProvidersManager>().UnregisterResourceProvider(provider);
    }

    public bool TryFindResource<T>(object key, out T? result)
    {
        return ServiceLocator.GetService<IResourceProvidersManager>().TryFindResource(key, out result);
    }

    #endregion
}
