using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class AcssLoader : IAcssLoader
{
    private readonly IAcssContext _context;

    public AcssLoader(IAcssContext acssContext)
    {
        _context = acssContext;
    }

    public void Initialize()
    {
        
    }

    IAcssFile? IAcssLoader.Load(Styles owner, string filePath, string? preferredPath, bool autoReloadWhenFileChanged)
    {
        var path = filePath;
        if (string.IsNullOrWhiteSpace(preferredPath) == false && File.Exists(preferredPath))
        {
            path = preferredPath;
        }
        
        path = path.GetStandardPath();
        if (_context.TryGetAcssFile(path, out var file))
        {
            return file;
        }

        if (File.Exists(path) == false)
        {
            this.WriteError($"Can not find acss file {path}. Skip it.");
            return null;
        }

        file = AcssFile.TryLoad(_context, owner, path, autoReloadWhenFileChanged);
        _context.TryAddAcssFile(file);
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
}
