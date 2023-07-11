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

    ICssFile? ICssLoader.Load(Styles styles, string filePath, bool autoReloadWhenFileChanged)
    {
        filePath = GetStandardPath(filePath);
        if (CssBuilder.TryGetCssFile(filePath, out _))
        {
            return null;
        }

        var file = CssFile.Load(CssBuilder, styles, filePath, autoReloadWhenFileChanged);
        CssBuilder.TryAddCssFile(file);
        return file;
    }

    ICssFile? ICssLoader.BeginLoad(Styles styles, string filePath, bool autoReloadWhenFileChanged)
    {
        filePath = GetStandardPath(filePath);
        if (CssBuilder.TryGetCssFile(filePath, out _))
        {
            return null;
        }

        var file = CssFile.BeginLoad(CssBuilder, styles, filePath, autoReloadWhenFileChanged);
        CssBuilder.TryAddCssFile(file);
        return file;
    }

    IEnumerable<ICssFile> ICssLoader.LoadFolder(Styles styles, string folderPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            throw new FileNotFoundException($"Can not find the folder '{folderPath}'.");
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((ICssLoader)this).Load(styles, f.FullName, autoReloadWhenFileChanged)).OfType<ICssFile>().ToList();
    }

    IEnumerable<ICssFile> ICssLoader.BeginLoadFolder(Styles styles, string folderPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            throw new FileNotFoundException($"Can not find the folder '{folderPath}'.");
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((ICssLoader)this).BeginLoad(styles, f.FullName, autoReloadWhenFileChanged)).OfType<ICssFile>().ToList();
    }

    ICssFile? ICssLoader.Load(Styles styles, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((ICssLoader)this).Load(styles, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).Load(styles, filePath, autoReloadWhenFileChanged);
#endif
    }

    ICssFile? ICssLoader.BeginLoad(Styles styles, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((ICssLoader)this).BeginLoad(styles, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).BeginLoad(styles, filePath, autoReloadWhenFileChanged);
#endif
    }

    IEnumerable<ICssFile> ICssLoader.LoadFolder(Styles styles, string folderPath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, folderPath);
        return ((ICssLoader)this).LoadFolder(styles, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).LoadFolder(styles, folderPath, autoReloadWhenFileChanged);
#endif
    }

    IEnumerable<ICssFile> ICssLoader.BeginLoadFolder(Styles styles, string folderPath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, folderPath);
        return ((ICssLoader)this).BeginLoadFolder(styles, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).BeginLoadFolder(styles, folderPath, autoReloadWhenFileChanged);
#endif
    }



    private static string GetStandardPath(string path)
    {
        return Path.GetFullPath(path).ToLower();
    }
}
