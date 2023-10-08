using System.Collections.Generic;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A loader that provides the abilities to load acss files.
/// </summary>
public interface IAcssLoader : IService
{
    /// <summary>
    /// Load an avalonia acss style from an acss source.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="source"></param>
    /// <param name="autoReload"></param>
    /// <returns></returns>
    public IAcssFile? Load(Styles owner, ISource source, bool autoReload = true);

    /// <summary>
    /// Load avalonia acss styles from an <see cref="ISourceCollection"/> synchronously.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="sourceCollection"></param>
    /// <param name="autoReload"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> LoadCollection(Styles owner, ISourceCollection sourceCollection, bool autoReload = true);
}