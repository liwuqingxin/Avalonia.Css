using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class AcssLoader : IAcssLoader
{
    public AcssLoader(IAcssBuilder acssBuilder)
    {
        AcssBuilder = acssBuilder;
    }

    public IAcssBuilder AcssBuilder { get; }

    IAcssFile? IAcssLoader.Load(Styles owner, string filePath, bool autoReloadWhenFileChanged)
    {
        filePath = GetStandardPath(filePath);
        if (AcssBuilder.TryGetAcssFile(filePath, out var file))
        {
            return file;
        }

        file = AcssFile.Load(AcssBuilder, owner, filePath, autoReloadWhenFileChanged);
        AcssBuilder.TryAddAcssFile(file);
        return file;
    }

    IAcssFile? IAcssLoader.BeginLoad(Styles owner, string filePath, bool autoReloadWhenFileChanged)
    {
        filePath = GetStandardPath(filePath);
        if (AcssBuilder.TryGetAcssFile(filePath, out var file))
        {
            return file;
        }

        file = AcssFile.BeginLoad(AcssBuilder, owner, filePath, autoReloadWhenFileChanged);
        AcssBuilder.TryAddAcssFile(file);
        return file;
    }

    IEnumerable<IAcssFile> IAcssLoader.LoadFolder(Styles owner, string folderPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            throw new FileNotFoundException($"Can not find the folder '{folderPath}'.");
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((IAcssLoader)this).Load(owner, f.FullName, autoReloadWhenFileChanged)).OfType<IAcssFile>().ToList();
    }

    IEnumerable<IAcssFile> IAcssLoader.BeginLoadFolder(Styles owner, string folderPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            throw new FileNotFoundException($"Can not find the folder '{folderPath}'.");
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((IAcssLoader)this).BeginLoad(owner, f.FullName, autoReloadWhenFileChanged)).OfType<IAcssFile>().ToList();
    }

    IAcssFile? IAcssLoader.Load(Styles owner, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((IAcssLoader)this).Load(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).Load(owner, filePath, autoReloadWhenFileChanged);
#endif
    }

    IAcssFile? IAcssLoader.BeginLoad(Styles owner, string filePath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, filePath);
        return ((IAcssLoader)this).BeginLoad(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).BeginLoad(owner, filePath, autoReloadWhenFileChanged);
#endif
    }

    IEnumerable<IAcssFile> IAcssLoader.LoadFolder(Styles owner, string folderPath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, folderPath);
        return ((IAcssLoader)this).LoadFolder(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).LoadFolder(owner, folderPath, autoReloadWhenFileChanged);
#endif
    }

    IEnumerable<IAcssFile> IAcssLoader.BeginLoadFolder(Styles owner, string folderPath, string debugRelative, bool autoReloadWhenFileChanged)
    {
#if DEBUG
        var path = Path.Combine(debugRelative, folderPath);
        return ((IAcssLoader)this).BeginLoadFolder(owner, path, autoReloadWhenFileChanged);
#else
        return ((ICssLoader)this).BeginLoadFolder(owner, folderPath, autoReloadWhenFileChanged);
#endif
    }



    private static string GetStandardPath(string path)
    {
        return Path.GetFullPath(path)/*.ToLower()*/;
    }
}
