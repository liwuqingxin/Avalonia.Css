using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nlnet.Avalonia.Css;

public class AcssContext : IAcssContext, IService
{
    #region Default IAcssContext

    private static int _prepared;
    private static IAcssContext? _default;

    public static IAcssContext Default
    {
        get
        {
            if (_default == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(AcssExtension)}.{nameof(AcssExtension.UseAcssDefaultContext)}() or " +
                    $"{nameof(AcssContext)}.{nameof(AcssContext.UseDefaultContext)}() " +
                    $"must be called before accessing the {nameof(AcssContext)}.{nameof(Default)}.");
            }
            return _default;
        }
    }

    /// <summary>
    /// Use default <see cref="IAcssContext"/>.
    /// </summary>
    /// <returns></returns>
    public static IAcssContext UseDefaultContext()
    {
        if (Interlocked.Exchange(ref _prepared, 1) == 0)
        {
            _default = new AcssContext();
        }

        if (_default is IService service)
        {
            service.Initialize();
        }

        return _default!;
    }

    #endregion



    private readonly ConcurrentDictionary<ISource, IAcssFile> _files = new();
    private readonly ConcurrentDictionary<ISource, AcssTokens> _tokens = new();

    internal AcssContext()
    {
        var services = this as IServiceProvider;

        // Diagnosis
        services.UseService<IDiagnosisOutput>(new TraceDiagnosisOutput());

        // Syntax
        services.UseService<IAcssInterpreter>(new AcssInterpreter(this));
        services.UseService<IAcssParser>(new AcssParser(this));

        // Resolver
        services.UseService<ITypeResolverManager>(new TypeResolverManager());
        services.UseService<IValueParsingTypeAdapterManager>(new ValueParsingTypeAdapterManager());
        services.UseService<IResourceProvidersManager>(new ResourceProvidersManager(this));
        services.UseService<IBehaviorDeclarerManager>(new BehaviorDeclarerManager());
        services.UseService<IBehaviorResolverManager>(new BehaviorResolverManager());

        // Factory
        services.UseService<IAcssResourceFactory>(new AcssResourceFactory(this));
        services.UseService<IAcssSectionFactory>(new AcssSectionFactory(this));
        services.UseService<ISourceFactory>(new DefaultSourceFactory());

        // Loader
        services.UseService<IAcssLoader>(new AcssLoader(this));

        // File Watcher
        services.UseService<IFileSourceMonitor>(new FileSourceMonitor());

        // Config
        services.UseService<IAcssConfiguration>(new AcssConfiguration());

        // Rider
        services.UseService<IRiderSettingsBuilder>(new RiderSettingsBuilder(this));
    }

    void IService.Initialize()
    {
        foreach (var service in _services)
        {
            service.Initialize();
        }
    }



    #region IAcssContext

    bool IAcssContext.TryAddAcssFile(IAcssFile file)
    {
        if (_files.TryGetValue(file.Source, out _))
        {
            return false;
        }

        _files.TryAdd(file.Source, file);
        return true;
    }

    bool IAcssContext.TryRemoveAcssFile(IAcssFile file)
    {
        return _files.TryRemove(file.Source, out _);
    }

    bool IAcssContext.TryGetAcssFile(ISource source, out IAcssFile? file)
    {
        if (_files.TryGetValue(source, out var f))
        {
            file = f;
            return true;
        }

        file = null;
        return false;
    }

    bool IAcssContext.TryAddAcssTokens(ISource source, AcssTokens tokens)
    {
        if (_tokens.TryGetValue(source, out _))
        {
            return false;
        }
        
        _tokens.TryAdd(source, tokens);
        return true;
    }

    bool IAcssContext.TryRemoveAcssTokens(ISource source, AcssTokens tokens)
    {
        return _tokens.TryRemove(source, out _);
    }

    bool IAcssContext.TryGetAcssTokens(ISource source, out AcssTokens? tokens)
    {
        if (_tokens.TryGetValue(source, out var t))
        {
            tokens = t;
            return true;
        }

        tokens = null;
        return false;
    }

    void IAcssContext.EnableTransitions(bool enable)
    {
        var cfg = AcssContext.Default.GetService<IAcssConfiguration>();
        cfg.EnableTransitions = enable;
        (this as IAcssContext).Reload();
    }

    void IAcssContext.Reload()
    {
        foreach (var file in _files.Values)
        {
            file.Reload(true);
        }
    }

    #endregion



    #region IServiceProvider

    private readonly IList<IService> _services = new List<IService>();
    private readonly IDictionary<Type, IService> _namedServices = new Dictionary<Type, IService>();

    void IServiceProvider.UseService(IService service)
    {
        _services.Add(service);
    }

    void IServiceProvider.UseService<T>()
    {
        _services.Add(new T());
    }

    void IServiceProvider.UseService<TService, TImpl>()
    {
        var serviceType = typeof(TService);
        if (_namedServices.TryGetValue(serviceType, out var old))
        {
            _services.Remove(old);
        }

        var impl = new TImpl();
        _services.Add(impl);
        _namedServices[serviceType] = impl;
    }

    public void UseService<TService>(TService service) where TService : IService
    {
        var serviceType = typeof(TService);
        if (_namedServices.TryGetValue(serviceType, out var old))
        {
            _services.Remove(old);
        }
        
        _services.Add(service);
        _namedServices[serviceType] = service;
    }

    T IServiceProvider.GetService<T>()
    {
        if (_services.FirstOrDefault(s => s is T) is not T service)
        {
            throw new Exception($"Service {typeof(T)} is not found.");
        }

        return service;
    }

    public T? GetServiceIfExist<T>() where T : class, IService
    {
        return _services.FirstOrDefault(s => s is T) is T service ? service : null;
    }

    bool IServiceProvider.TryGetService<T>(out T service) where T : class
    {
#pragma warning disable CS8601
        service = _services.FirstOrDefault(s => s is T) as T;
#pragma warning restore CS8601
        return service != null;
    }

    IEnumerable<T> IServiceProvider.GetServices<T>()
    {
        return _services.OfType<T>();
    }

    #endregion
}