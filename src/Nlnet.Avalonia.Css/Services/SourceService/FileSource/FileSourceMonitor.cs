using System;

using System.Collections.Concurrent;
using System.IO;

namespace Nlnet.Avalonia.Css;

public interface IFileSourceMonitor : IService
{
    public void AddSource(ISource source);

    public void RemoveSource(ISource source);
}

public class FileSourceMonitor : IFileSourceMonitor
{
    private readonly IAcssContext _acssContext;
    private readonly ConcurrentDictionary<string, ISource> _sources = new();
    private readonly ConcurrentDictionary<ISource, FileSystemWatcher> _monitors = new();

    public FileSourceMonitor(IAcssContext acssContext)
    {
        _acssContext = acssContext;
    }

    void IService.Initialize()
    {
        
    }

    public void AddSource(ISource source)
    {
        var keyPath = source.GetKeyPath();
        var dir     = Path.GetDirectoryName(keyPath);

        if (Directory.Exists(dir) == false)
        {
            this.WriteError($"Directory not exist : '{dir}'. Skip to monitor it.");
            return;
        }

        if (_monitors.TryGetValue(source, out var watcher) == false)
        {
            watcher = MonitorDirectory(dir);
            _monitors.TryAdd(source, watcher);
        }

        watcher.Filters.Add(Path.GetFileName(keyPath));

        _sources[keyPath] = source;
    }

    public void RemoveSource(ISource source)
    {
        if (_monitors.TryGetValue(source, out var watcher))
        {
            watcher.Filters.Remove(Path.GetFileName(source.GetKeyPath()));
            if (watcher.Filters.Count == 0)
            {
                watcher.Dispose();
                _monitors.TryRemove(source, out _);
            }
        }
    }

    private FileSystemWatcher MonitorDirectory(string dir)
    {
        var watcher = new FileSystemWatcher(dir)
        {
            EnableRaisingEvents = true,
            NotifyFilter        = NotifyFilters.LastWrite,
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

        if (_sources.TryGetValue(e.FullPath.GetStandardPath(), out var source))
        {
            source.OnSourceChanged();
        }
    }
}