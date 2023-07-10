using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class CssLoader : ICssLoader
{
    public ICssBuilder CssBuilder { get; }

    public CssLoader(ICssBuilder cssBuilder)
    {
        CssBuilder = cssBuilder;
    }

    public ICssFile Load(Styles styles, string filePath, bool autoReloadWhenFileChanged = true)
    {
        return CssFile.Load(CssBuilder, styles, filePath, autoReloadWhenFileChanged);
    }
    
    public ICssFile BeginLoad(Styles styles, string filePath, bool autoReloadWhenFileChanged = true)
    {
        return CssFile.BeginLoad(CssBuilder, styles, filePath, autoReloadWhenFileChanged);
    }
}
