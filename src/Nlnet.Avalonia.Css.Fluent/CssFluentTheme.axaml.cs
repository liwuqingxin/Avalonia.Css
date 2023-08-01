using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Nlnet.Avalonia.Css.Controls;
using Nlnet.Avalonia.Senior.Controls;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class CssFluentTheme : Styles
    {
        private ICssFile? _modeFile;
        private ICssFile? _themeFile;
        private ICssFile? _resourceFile;
        
        static CssFluentTheme()
        {
            TemplatedControlExtension.Init();
        }

        public CssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);
            Load();
        }

        private void Load()
        {
            CssBuilder.UseDefaultBuilder();

            // This is not added to application's styles till now. Register this to resource manager to enable resource access to this.
            CssBuilder.Default.ResourceProvidersManager.RegisterResourceProvider(this);

            // Nlnet.Avalonia.Css.Controls
            CssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<AnimatingPresenter>());

            // Nlnet.Avalonia.Senior
            CssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<NtScrollViewer>());

            var loader = CssBuilder.Default.BuildLoader();

            const string debugRelative = "../../../Nlnet.Avalonia.Css.Fluent/";

            _modeFile     = loader.Load(this, "Css/Resources/Mode.acss",      debugRelative);
            _themeFile    = loader.Load(this, "Css/Resources/Theme.acss",     debugRelative);
            _resourceFile = loader.Load(this, "Css/Resources/Resources.acss", debugRelative);

            loader.LoadFolder(this, "Css/", debugRelative);
            loader.LoadFolder(this, "Css/Senior", debugRelative);
        }

        public void UpdateResource(bool reapplyStyle)
        {
            _resourceFile?.Reload(reapplyStyle);
        }

        public void UpdateMode(bool reapplyStyle)
        {
            _modeFile?.Reload(reapplyStyle);
        }

        public void UpdateTheme(bool reapplyStyle)
        {
            _themeFile?.Reload(reapplyStyle);
        }
    }
}
