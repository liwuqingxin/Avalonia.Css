using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform;
using Path = System.IO.Path;

namespace Nlnet.Avalonia.Css;

public class EmbeddedSourceCollection : ISourceCollection
{
    private readonly Uri _baseUri;

    public EmbeddedSourceCollection(Uri baseUri)
    {
        _baseUri = baseUri;
    }

    IEnumerable<ISource> ISourceCollection.GetSources()
    {
        var assets = AssetLoader.GetAssets(_baseUri, null).ToList();
        if (assets.Count == 0)
        {
            this.WriteError($"Can not find the any acss resources under uri '{_baseUri}'. Skip it.");
            return Enumerable.Empty<ISource>();
        }

        var fileSources = assets
                          .Where(u => string.Equals(Path.GetExtension(u.AbsolutePath), ".acss",
                              StringComparison.InvariantCultureIgnoreCase))
                          .Select(u => new EmbeddedSource(u));

        return fileSources;
    }

    bool ISourceCollection.IsValid()
    {
        return AssetLoader.GetAssets(_baseUri, null).Any();
    }
}