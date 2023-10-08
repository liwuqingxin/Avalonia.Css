using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nlnet.Avalonia.Css;

public class FileSourceCollection : ISourceCollection
{
    private readonly string  _folder;
    private readonly string? _preferFolder;

    public FileSourceCollection(string folder)
    {
        _folder = folder;
    }

    public FileSourceCollection(string folder, string preferFolder)
    {
        _folder = folder;
        _preferFolder = preferFolder;
    }

    IEnumerable<ISource> ISourceCollection.GetSources()
    {
        var path = string.Empty;
        if (Directory.Exists(_preferFolder))
        {
            path = _preferFolder;
        }
        else if (Directory.Exists(_folder))
        {
            path = _folder;
        }
        else
        {
            this.WriteError($"Can not find the folder '{path}'. Skip it.");
            return Enumerable.Empty<ISource>();
        }

        var files = new DirectoryInfo(path)
                    .GetFiles()
                    .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase))
                    .Select(f => new FileSource(f.FullName));

        return files;
    }

    bool ISourceCollection.IsValid()
    {
        return Directory.Exists(_preferFolder) || Directory.Exists(_folder);
    }
}