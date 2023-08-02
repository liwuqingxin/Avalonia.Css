using System.Collections.Generic;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// A loader that provides the abilities to load acss files.
/// </summary>
public interface IAcssLoader
{
    /// <summary>
    /// The acss builder instance.
    /// </summary>
    public IAcssBuilder AcssBuilder { get; }

    /// <summary>
    /// Load a avalonia acss style from an acss file synchronously.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="filePath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IAcssFile? Load(Styles owner, string filePath, bool autoReloadWhenFileChanged = true);
    
    /// <summary>
    /// Load a avalonia acss style from an acss file asynchronously.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="filePath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IAcssFile? BeginLoad(Styles owner, string filePath, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia acss styles from an folder synchronously. Note that this does not load recursively.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="folderPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> LoadFolder(Styles owner, string folderPath, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia acss styles from an folder synchronously. Note that this does not load recursively.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="folderPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> BeginLoadFolder(Styles owner, string folderPath, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load a avalonia acss style from an acss file with relative path of debug synchronously.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="filePath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IAcssFile? Load(Styles owner, string filePath, string debugRelative, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load a avalonia acss style from an acss file with relative path of debug asynchronously.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="filePath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IAcssFile? BeginLoad(Styles owner, string filePath, string debugRelative, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia acss styles from an folder synchronously. Note that this does not load recursively.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="folderPath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> LoadFolder(Styles owner, string folderPath, string debugRelative, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia acss styles from an folder synchronously. Note that this does not load recursively.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="folderPath"></param>
    /// <param name="debugRelative"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> BeginLoadFolder(Styles owner, string folderPath, string debugRelative, bool autoReloadWhenFileChanged = true);
}