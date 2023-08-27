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

    IAcssFile? IAcssLoader.Load(Styles owner, string filePath, string? optionalSyncPath, bool autoReloadWhenFileChanged)
    {
        filePath = filePath.GetStandardPath();
        if (AcssBuilder.TryGetAcssFile(filePath, out var file))
        {
            return file;
        }

        if (File.Exists(filePath) == false)
        {
            this.WriteError($"Can not find acss file {filePath}. Skip it.");
            return null;
        }

        file = AcssFile.TryLoad(AcssBuilder, owner, filePath, optionalSyncPath, autoReloadWhenFileChanged);
        AcssBuilder.TryAddAcssFile(file);
        return file;
    }

    IAcssFile? IAcssLoader.BeginLoad(Styles owner, string filePath, string? optionalSyncPath, bool autoReloadWhenFileChanged)
    {
        filePath = filePath.GetStandardPath();
        if (AcssBuilder.TryGetAcssFile(filePath, out var file))
        {
            return file;
        }

        if (File.Exists(filePath) == false)
        {
            this.WriteError($"Can not find acss file {filePath}. Skip it.");
            return null;
        }

        file = AcssFile.TryBeginLoad(AcssBuilder, owner, filePath, optionalSyncPath, autoReloadWhenFileChanged);
        AcssBuilder.TryAddAcssFile(file);
        return file;
    }

    IEnumerable<IAcssFile> IAcssLoader.LoadFolder(Styles owner, string folderPath, string? optionalSyncPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            this.WriteError($"Can not find the folder '{folderPath}'. Skip it.");
            return Enumerable.Empty<IAcssFile>();
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((IAcssLoader)this).Load(owner, f.FullName, optionalSyncPath, autoReloadWhenFileChanged)).OfType<IAcssFile>().ToList();
    }

    IEnumerable<IAcssFile> IAcssLoader.BeginLoadFolder(Styles owner, string folderPath, string? optionalSyncPath, bool autoReloadWhenFileChanged)
    {
        if (Directory.Exists(folderPath) == false)
        {
            this.WriteError($"Can not find the folder '{folderPath}'. Skip it.");
            return Enumerable.Empty<IAcssFile>();
        }

        var files = new DirectoryInfo(folderPath)
            .GetFiles()
            .Where(f => string.Equals(f.Extension, ".acss", StringComparison.InvariantCultureIgnoreCase));

        return files.Select(f => ((IAcssLoader)this).BeginLoad(owner, f.FullName, optionalSyncPath, autoReloadWhenFileChanged)).OfType<IAcssFile>().ToList();
    }
}
