﻿using System;

using System.Collections.Concurrent;
using System.IO;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A file monitor that can provide file change notifications.
/// </summary>
public interface IFileSourceMonitor : IService
{
    /// <summary>
    /// Add a source that should be monitored.
    /// </summary>
    /// <param name="source"></param>
    public void Monitor(ISource source);

    /// <summary>
    /// Stop monitor for the <see cref="source"/>.
    /// </summary>
    /// <param name="source"></param>
    public void StopMonitor(ISource source);
}

public class FileSourceMonitor : IFileSourceMonitor
{
    private readonly ConcurrentDictionary<string, ISource> _sources = new();
    private readonly ConcurrentDictionary<ISource, FileSystemWatcher> _monitors = new();

    public FileSourceMonitor()
    {
        
    }

    void IService.Initialize()
    {
        
    }

    public void Monitor(ISource source)
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

    public void StopMonitor(ISource source)
    {
        if (!_monitors.TryGetValue(source, out var watcher))
        {
            return;
        }

        watcher.Filters.Remove(Path.GetFileName(source.GetKeyPath()));
        if (watcher.Filters.Count == 0)
        {
            watcher.Dispose();
            _monitors.TryRemove(source, out _);
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