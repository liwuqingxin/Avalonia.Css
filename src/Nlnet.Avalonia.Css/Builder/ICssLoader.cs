using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A loader that provides the abilities to load css files.
/// </summary>
public interface ICssLoader
{
    /// <summary>
    /// Load a avalonia css style from an css file synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public ICssFile Load(Styles styles, string filePath, bool autoReloadWhenFileChanged = true);
    
    /// <summary>
    /// Load a avalonia css style from an css file asynchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public ICssFile BeginLoad(Styles styles, string filePath, bool autoReloadWhenFileChanged = true);
}