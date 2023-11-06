using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Source collection.
/// </summary>
public interface ISourceCollection
{
    /// <summary>
    /// Get all source.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ISource> GetSources();

    /// <summary>
    /// Check if this source collection is valid.
    /// </summary>
    /// <returns></returns>
    bool IsValid();
}