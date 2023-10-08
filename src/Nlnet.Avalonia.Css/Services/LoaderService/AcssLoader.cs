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

    void IService.Initialize()
    {
        
    }

    IAcssFile? IAcssLoader.Load(Styles owner, ISource source, bool autoReload)
    {
        if (source.IsValid() == false)
        {
            return null;
        }

        // TODO Check context but owner here. Should fix it?
        if (_context.TryGetAcssFile(source, out var file))
        {
            return file;
        }

        file = AcssFile.TryLoad(_context, owner, source, autoReload);
        _context.TryAddAcssFile(file);
        return file;
    }

    IEnumerable<IAcssFile> IAcssLoader.LoadCollection(Styles owner, ISourceCollection sourceCollection, bool autoReload)
    {
        if (sourceCollection.IsValid() == false)
        {
            return Enumerable.Empty<IAcssFile>();
        }

        var sources = sourceCollection.GetSources();
        return sources.Select(s => ((IAcssLoader)this).Load(owner, s, autoReload)).OfType<IAcssFile>().ToList();
    }
}
