using System.Collections.Generic;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A loader that provides the abilities to load css files.
/// </summary>
public interface ICssLoader
{
    /// <summary>
    /// The css builder instance.
    /// </summary>
    public ICssBuilder CssBuilder { get; }

    /// <summary>
    /// Load a avalonia css style from an css file synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public ICssFile? Load(Styles styles, string filePath, bool autoReloadWhenFileChanged = true);
    
    /// <summary>
    /// Load a avalonia css style from an css file asynchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public ICssFile? BeginLoad(Styles styles, string filePath, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia css styles from an folder synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="folderPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<ICssFile> LoadFolder(Styles styles, string folderPath, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia css styles from an folder synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="folderPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<ICssFile> BeginLoadFolder(Styles styles, string folderPath, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load a avalonia css style from an css file with relative path of debug synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public ICssFile? Load(Styles styles, string filePath, string debugRelative, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load a avalonia css style from an css file with relative path of debug asynchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public ICssFile? BeginLoad(Styles styles, string filePath, string debugRelative, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia css styles from an folder synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="folderPath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<ICssFile> LoadFolder(Styles styles, string folderPath, string debugRelative, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia css styles from an folder synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="folderPath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<ICssFile> BeginLoadFolder(Styles styles, string folderPath, string debugRelative, bool autoReloadWhenFileChanged = true);
}