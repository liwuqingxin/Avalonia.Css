using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Nlnet.Avalonia.Css.Controls;
using Nlnet.Avalonia.Senior.Controls;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class AcssFluentTheme : Styles
    {
        private IAcssFile? _modeFile;
        private IAcssFile? _themeFile;
        private IAcssFile? _resourceFile;
        
        static AcssFluentTheme()
        {
            TemplatedControlExtension.Init();
        }

        public AcssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);
            Load();
        }

        private void Load()
        {
            AcssBuilder.UseDefaultBuilder();

            // This is not added to application's styles till now. Register this to resource manager to enable resource access to this.
            AcssBuilder.Default.ResourceProvidersManager.RegisterResourceProvider(this);

            // Nlnet.Avalonia.Css.Controls
            AcssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<AnimatingPresenter>());

            // Nlnet.Avalonia.Senior
            AcssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<NtScrollViewer>());

            var loader = AcssBuilder.Default.BuildLoader();

            const string debugRelative = "../../../Nlnet.Avalonia.Css.Fluent/";

            _modeFile     = loader.Load(this, "Acss/Resources/Mode.acss",      debugRelative);
            _themeFile    = loader.Load(this, "Acss/Resources/Theme.acss",     debugRelative);
            _resourceFile = loader.Load(this, "Acss/Resources/Resources.acss", debugRelative);

            loader.LoadFolder(this, "Acss/", debugRelative);
            loader.LoadFolder(this, "Acss/Senior", debugRelative);
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
