using System.IO;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Nlnet.Avalonia.Css.Controls;

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
            CssBuilder.Default.RegisterResourceProvider(this);

            // Nlnet.Avalonia.Css.Controls
            CssBuilder.Default.LoadResolver(new GenericResolver<AnimatingContainer>());

            var loader = CssBuilder.Default.BuildLoader();

            _modeFile     = loader.Load(this, "../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Mode.acss");
            _themeFile    = loader.Load(this, "../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Theme.acss");
            _resourceFile = loader.Load(this, "../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Resources.acss");

            var files = Directory.GetFiles("../../../Nlnet.Avalonia.Css.Fluent/Css/");
            foreach (var file in files)
            {
                loader.Load(this, file);
            }
        }

        public void UpdateResource()
        {
            _resourceFile?.Reload();
        }

        public void UpdateMode()
        {
            _modeFile?.Reload();
        }

        public void UpdateTheme()
        {
            _themeFile?.Reload();
        }
    }
}
