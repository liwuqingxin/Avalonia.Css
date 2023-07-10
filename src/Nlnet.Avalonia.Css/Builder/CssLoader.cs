using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class CssLoader : ICssLoader
{
    public ICssFile Load(Styles styles, string filePath, bool autoReloadWhenFileChanged = true)
    {
        return CssFile.Load(styles, filePath, autoReloadWhenFileChanged);
    }
    
    public ICssFile BeginLoad(Styles styles, string filePath, bool autoReloadWhenFileChanged = true)
    {
        return CssFile.BeginLoad(styles, filePath, autoReloadWhenFileChanged);
    }
}
