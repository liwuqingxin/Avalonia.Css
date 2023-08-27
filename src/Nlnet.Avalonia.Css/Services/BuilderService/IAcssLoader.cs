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
    /// Load an avalonia acss style from an acss file. If the <see cref="optionalSyncPath"/> is specified, the file will
    /// be synchronized to that path when the origin file changed.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="filePath"></param>
    /// <param name="optionalSyncPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IAcssFile? Load(Styles owner, string filePath, string? optionalSyncPath = null, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Begin loading an avalonia acss style from an acss file. If the <see cref="optionalSyncPath"/> is specified, the
    /// file will be synchronized to that path when the origin file changed.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="filePath"></param>
    /// <param name="optionalSyncPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IAcssFile? BeginLoad(Styles owner, string filePath, string? optionalSyncPath= null, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Load avalonia acss styles from an folder synchronously. Note that this does not load recursively.
    /// If the <see cref="optionalSyncPath"/> is specified, the file will be synchronized to that path when the origin
    /// file changed.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="folderPath"></param>
    /// <param name="optionalSyncPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> LoadFolder(Styles owner, string folderPath, string? optionalSyncPath= null, bool autoReloadWhenFileChanged = true);

    /// <summary>
    /// Begin loading avalonia acss styles from an folder synchronously. Note that this does not load recursively.
    /// If the <see cref="optionalSyncPath"/> is specified, the file will be synchronized to that path when the origin
    /// file changed.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="folderPath"></param>
    /// <param name="optionalSyncPath"></param>
    /// <param name="autoReloadWhenFileChanged"></param>
    /// <returns></returns>
    public IEnumerable<IAcssFile> BeginLoadFolder(Styles owner, string folderPath, string? optionalSyncPath= null, bool autoReloadWhenFileChanged = true);
}