using System.Collections.Generic;
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

    IAcssFile? IAcssLoader.Load(Styles owner, ISource source)
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

        file = AcssFile.TryLoad(_context, owner, source);
        _context.TryAddAcssFile(file);
        return file;
    }

    IEnumerable<IAcssFile> IAcssLoader.LoadCollection(Styles owner, ISourceCollection sourceCollection)
    {
        if (sourceCollection.IsValid() == false)
        {
            return Enumerable.Empty<IAcssFile>();
        }

        var sources = sourceCollection.GetSources();
        return sources.Select(s => ((IAcssLoader)this).Load(owner, s)).OfType<IAcssFile>().ToList();
    }
}
