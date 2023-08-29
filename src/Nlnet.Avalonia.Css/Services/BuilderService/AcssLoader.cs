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

    IAcssFile? IAcssLoader.Load(Styles owner, string filePath, string? preferredPath, bool autoReloadWhenFileChanged)
    {
        var path = filePath;
        if (string.IsNullOrWhiteSpace(preferredPath) == false && File.Exists(preferredPath))
        {
            path = preferredPath;
        }
        
        path = path.GetStandardPath();
        if (AcssBuilder.TryGetAcssFile(path, out var file))
        {
            return file;
        }

        if (File.Exists(path) == false)
        {
            this.WriteError($"Can not find acss file {path}. Skip it.");
            return null;
        }

        file = AcssFile.TryLoad(AcssBuilder, owner, path, autoReloadWhenFileChanged);
        AcssBuilder.TryAddAcssFile(file);
        return file;
    }

    IAcssFile? IAcssLoader.BeginLoad(Styles owner, string filePath, string? preferredPath, bool autoReloadWhenFileChanged)
    {
        var path = filePath;
        if (string.IsNullOrWhiteSpace(preferredPath) == false && File.Exists(preferredPath))
        {
            path = preferredPath;
        }
        
        path = path.GetStandardPath();
        if (AcssBuilder.TryGetAcssFile(path, out var file))
        {
            return file;
        }

        if (File.Exists(path) == false)
        {
            this.WriteError($"Can not find acss file {path}. Skip it.");
            return null;
        }

        file = AcssFile.TryBeginLoad(AcssBuilder, owner, path, autoReloadWhenFileChanged);
        AcssBuilder.TryAddAcssFile(file);
        return file;
    }

    IEnumerable<IAcssFile> IAcssLoader.LoadFolder(Styles owner, string folderPath, string? preferredPath, bool autoReloadWhenFileChanged)
    {
        var path = folderPath;
        if (string.IsNullOrWhiteSpace(preferredPath) == false && Directory.Exists(preferredPath))
        {
            path = preferredPath;
        }
        
        if (Directory.Exists(path) == false)
        {
            this.WriteError($"Can not find the folder '{path}'. Skip it.");
            return Enumerable.Empty<IAcssFile>();
        }

        var files = new DirectoryInfo(path)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((IAcssLoader)this).Load(owner, f.FullName, preferredPath, autoReloadWhenFileChanged)).OfType<IAcssFile>().ToList();
    }

    IEnumerable<IAcssFile> IAcssLoader.BeginLoadFolder(Styles owner, string folderPath, string? preferredPath, bool autoReloadWhenFileChanged)
    {
        var path = folderPath;
        if (string.IsNullOrWhiteSpace(preferredPath) == false && Directory.Exists(preferredPath))
        {
            path = preferredPath;
        }
        
        if (Directory.Exists(path) == false)
        {
            this.WriteError($"Can not find the folder '{path}'. Skip it.");
            return Enumerable.Empty<IAcssFile>();
        }

        var files = new DirectoryInfo(path)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((IAcssLoader)this).BeginLoad(owner, f.FullName, preferredPath, autoReloadWhenFileChanged)).OfType<IAcssFile>().ToList();
    }
}
