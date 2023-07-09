using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public static class CssFileExtensions
{
    /// <summary>
    /// Load a avalonia css style from an acss file synchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="autoLoadWhenFileChanged"></param>
    /// <returns></returns>
    public static CssFile LoadCssFile(this Styles styles, string filePath, bool autoLoadWhenFileChanged)
    {
        return CssFile.Load(styles, filePath, autoLoadWhenFileChanged);
    }

    /// <summary>
    /// Load a avalonia css style from an acss file asynchronously.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="filePath"></param>
    /// <param name="autoLoadWhenFileChanged"></param>
    /// <returns></returns>
    public static CssFile BeginLoadCssFile(this Styles styles, string filePath, bool autoLoadWhenFileChanged)
    {
        return CssFile.BeginLoad(styles, filePath, autoLoadWhenFileChanged);
    }
}
