using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
    /// Get the first service of <see cref="T"/> if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetService<T>() where T : class, IService;

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
    /// <param name="standardFilePath"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryGetAcssFile(string standardFilePath, out IAcssFile? file);

    /// <summary>
    /// Try to add an acss tokens to the context.
    /// </summary>
    /// <param name="standardFilePath"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryAddAcssTokens(string standardFilePath, AcssTokens tokens);

    /// <summary>
    /// Try to remove an acss tokens to the context.
    /// </summary>
    /// <param name="standardFilePath"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryRemoveAcssTokens(string standardFilePath, AcssTokens tokens);

    /// <summary>
    /// Try to get an acss tokens from the context.
    /// </summary>
    /// <param name="standardFilePath"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryGetAcssTokens(string standardFilePath, out AcssTokens? tokens);
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
    
    private readonly ConcurrentDictionary<string, IAcssFile> _files = new();
    private readonly ConcurrentDictionary<string, AcssTokens> _tokens = new();
    private readonly ConcurrentDictionary<string, FileSystemWatcher> _monitors = new();

    internal AcssContext()
    {
        AddService(new AcssInterpreter(this));
        AddService(new AcssParser(this));

        AddService(new TypeResolverManager());
        AddService(new ValueParsingTypeAdapterManager());
        AddService(new ResourceProvidersManager());
        AddService(new BehaviorDeclarerManager());
        AddService(new BehaviorResolverManager());

        AddService(new AcssResourceFactory(this));
        AddService(new AcssSectionFactory(this));
        AddService(new AcssLoader(this));

        AddService(new AcssConfiguration());
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
        if (_files.TryGetValue(file.StandardFilePath, out _))
        {
            return false;
        }

        _files.TryAdd(file.StandardFilePath, file);
        return true;
    }

    bool IAcssContext.TryRemoveAcssFile(IAcssFile file)
    {
        return _files.TryRemove(file.StandardFilePath, out _);
    }

    bool IAcssContext.TryGetAcssFile(string standardFilePath, out IAcssFile? file)
    {
        if (_files.TryGetValue(standardFilePath, out var f))
        {
            file = f;
            return true;
        }

        file = null;
        return false;
    }

    bool IAcssContext.TryAddAcssTokens(string standardFilePath, AcssTokens tokens)
    {
        if (_tokens.TryGetValue(standardFilePath, out _))
        {
            return false;
        }

        var dir = Path.GetDirectoryName(standardFilePath);
        if (dir == null)
        {
            throw new InvalidOperationException($"Can not get the directory from path '{standardFilePath}'.");
        }

        if (_monitors.TryGetValue(dir, out var watcher) == false)
        {
            watcher = MonitorDirectory(dir);
            _monitors.TryAdd(dir, watcher);
        }

        watcher.Filters.Add(Path.GetFileName(standardFilePath));

        _tokens.TryAdd(standardFilePath, tokens);
        return true;
    }

    bool IAcssContext.TryRemoveAcssTokens(string standardFilePath, AcssTokens tokens)
    {
        var dir = Path.GetDirectoryName(standardFilePath);
        if (dir != null && _monitors.TryGetValue(dir, out var watcher))
        {
            watcher.Filters.Remove(Path.GetFileName(standardFilePath));
            if (watcher.Filters.Count == 0)
            {
                watcher.Dispose();
                _monitors.TryRemove(dir, out _);
            }
        }

        return _tokens.TryRemove(standardFilePath, out _);
    }

    bool IAcssContext.TryGetAcssTokens(string standardFilePath, out AcssTokens? tokens)
    {
        if (_tokens.TryGetValue(standardFilePath, out var t))
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

    IEnumerable<T> IServiceProvider.GetServices<T>()
    {
        return _services.OfType<T>();
    }

    #endregion



    #region Private

    private FileSystemWatcher MonitorDirectory(string dir)
    {
        var watcher = new FileSystemWatcher(dir)
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite,
        };

        watcher.Changed += OnFileChanged;

        return watcher;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        if (_tokens.TryGetValue(e.FullPath.GetStandardPath(), out var tokens))
        {
            tokens.OnFileChanged();
        }
    }

    #endregion

}