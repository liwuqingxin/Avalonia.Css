using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A service definition.
/// </summary>
public interface IService
{
    public void Initialize();
}

public interface IServiceProvider
{
    /// <summary>
    /// Get the first service of <see cref="T"/>.  If it does not exist, exception will be thrown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetService<T>() where T : class, IService;

    /// <summary>
    /// Try to get the first service of <see cref="T"/> if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <returns></returns>
    public bool TryGetService<T>(out T? service) where T : class, IService;

    /// <summary>
    /// Get all services of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> GetServices<T>() where T : class, IService;
}

public interface IAcssContext : IServiceProvider
{
    /// <summary>
    /// Try to add an acss file to the context.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryAddAcssFile(IAcssFile file);

    /// <summary>
    /// Try to remove an acss file from the context.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryRemoveAcssFile(IAcssFile file);

    /// <summary>
    /// Try to get an acss file from the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryGetAcssFile(ISource source, out IAcssFile? file);

    /// <summary>
    /// Try to add an acss tokens to the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryAddAcssTokens(ISource source, AcssTokens tokens);

    /// <summary>
    /// Try to remove an acss tokens to the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryRemoveAcssTokens(ISource source, AcssTokens tokens);

    /// <summary>
    /// Try to get an acss tokens from the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryGetAcssTokens(ISource source, out AcssTokens? tokens);
}

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



    private readonly List<IService> _services = new();

    private readonly ConcurrentDictionary<ISource, IAcssFile> _files = new();
    private readonly ConcurrentDictionary<ISource, AcssTokens> _tokens = new();

    internal AcssContext()
    {
        // Syntax
        AddService(new AcssInterpreter(this));
        AddService(new AcssParser(this));

        // Resolver
        AddService(new TypeResolverManager());
        AddService(new ValueParsingTypeAdapterManager());
        AddService(new ResourceProvidersManager());
        AddService(new BehaviorDeclarerManager());
        AddService(new BehaviorResolverManager());

        // Factory
        AddService(new AcssResourceFactory(this));
        AddService(new AcssSectionFactory(this));
        AddService(new DefaultSourceFactory());

        // Loader
        AddService(new AcssLoader(this));

        // File Watcher
        AddService(new FileSourceMonitor());

        // Config
        AddService(new AcssConfiguration());

        // Rider
        AddService(new RiderSettingsBuilder(this));
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
    
    #endregion



    #region IServiceProvider

    public void AddService(IService service)
    {
        _services.Add(service);
    }

    T IServiceProvider.GetService<T>()
    {
        if (_services.FirstOrDefault(s => s is T) is not T service)
        {
            throw new Exception($"Service {typeof(T)} is not found.");
        }

        return service;
    }

    public bool TryGetService<T>(out T? service) where T : class, IService
    {
        service = _services.FirstOrDefault(s => s is T) as T;
        return service != null;
    }

    IEnumerable<T> IServiceProvider.GetServices<T>()
    {
        return _services.OfType<T>();
    }

    #endregion
}