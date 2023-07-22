using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class CssLoader : ICssLoader
{
    public CssLoader(ICssBuilder cssBuilder)
    {
        CssBuilder = cssBuilder;
    }

    public ICssBuilder CssBuilder { get; }

    ICssFile? ICssLoader.Load(Styles owner, string filePath, bool autoReloadWhenFileChanged)
    {
        filePath = GetStandardPath(filePath);
        if (CssBuilder.TryGetCssFile(filePath, out var file))
        {
            return file;
        }

        file = CssFile.Load(CssBuilder, owner, filePath, autoReloadWhenFileChanged);
        CssBuilder.TryAddCssFile(file);
        return file;
    }

    ICssFile? ICssLoader.BeginLoad(Styles owner, string filePath, bool autoReloadWhenFileChanged)
    {
        filePath = GetStandardPath(filePath);
        if (CssBuilder.TryGetCssFile(filePath, out var file))
        {
            return file;
        }

        file = CssFile.BeginLoad(CssBuilder, owner, filePath, autoReloadWhenFileChanged);
        CssBuilder.TryAddCssFile(file);
        return file;
    }

    IEnumerable<ICssFile> ICssLoader.LoadFolder(Styles owner, string folderPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            throw new FileNotFoundException($"Can not find the folder '{folderPath}'.");
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((ICssLoader)this).Load(owner, f.FullName, autoReloadWhenFileChanged)).OfType<ICssFile>().ToList();
    }

    IEnumerable<ICssFile> ICssLoader.BeginLoadFolder(Styles owner, string folderPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            throw new FileNotFoundException($"Can not find the folder '{folderPath}'.");
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((ICssLoader)this).BeginLoad(owner, f.FullName, autoReloadWhenFileChanged)).OfType<ICssFile>().ToList();
    }

    ICssFile? ICssLoader.Load(Styles owner, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((ICssLoader)this).Load(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).Load(owner, filePath, autoReloadWhenFileChanged);
#endif
    }

    ICssFile? ICssLoader.BeginLoad(Styles owner, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((ICssLoader)this).BeginLoad(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).BeginLoad(owner, filePath, autoReloadWhenFileChanged);
#endif
    }

    IEnumerable<ICssFile> ICssLoader.LoadFolder(Styles owner, string folderPath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, folderPath);
        return ((ICssLoader)this).LoadFolder(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).LoadFolder(owner, folderPath, autoReloadWhenFileChanged);
#endif
    }

    IEnumerable<ICssFile> ICssLoader.BeginLoadFolder(Styles owner, string folderPath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, folderPath);
        return ((ICssLoader)this).BeginLoadFolder(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).BeginLoadFolder(owner, folderPath, autoReloadWhenFileChanged);
#endif
    }



    private static string GetStandardPath(string path)
    {
        return Path.GetFullPath(path).ToLower();
    }
}
