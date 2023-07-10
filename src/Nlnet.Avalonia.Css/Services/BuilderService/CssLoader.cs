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

    ICssFile ICssLoader.Load(Styles styles, string filePath, bool autoReloadWhenFileChanged)
    {
        return CssFile.Load(CssBuilder, styles, filePath, autoReloadWhenFileChanged);
    }

    ICssFile ICssLoader.BeginLoad(Styles styles, string filePath, bool autoReloadWhenFileChanged)
    {
        return CssFile.BeginLoad(CssBuilder, styles, filePath, autoReloadWhenFileChanged);
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
        
        foreach (var file in files)
        {
            yield return CssFile.Load(CssBuilder, styles, file.FullName, autoReloadWhenFileChanged);
        }
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

        foreach (var file in files)
        {
            yield return CssFile.BeginLoad(CssBuilder, styles, file.FullName, autoReloadWhenFileChanged);
        }
    }

    ICssFile ICssLoader.Load(Styles styles, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((ICssLoader)this).Load(styles, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).Load(styles, filePath, autoReloadWhenFileChanged);
#endif
    }

    ICssFile ICssLoader.BeginLoad(Styles styles, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
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
}
