using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Nlnet.Avalonia.Controls;
using Nlnet.Avalonia.Css.Controls;
using Nlnet.Avalonia.Senior.Controls;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class AcssFluentTheme : Styles
    {
        private IAcssFile? _accentColorFile;
        private IAcssFile? _accentFile;
        private IAcssFile? _themeFile;
        private IAcssFile? _transitions;
        
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
            AcssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<NotifyChangeContentPresenter>());
            
            // Avalonia.Controls.DataGrid
            AcssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<DataGrid>());

            // Nlnet.Avalonia.Senior
            AcssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<NtScrollViewer>());
            
            // Nlnet.Avalonia.MessageBox
            AcssBuilder.Default.TypeResolver.LoadResolver(new GenericTypeResolver<MessageBox>());

            var loader = AcssBuilder.Default.BuildLoader();

            const string debugRelative = "../../src/Nlnet.Avalonia.Css.Fluent/";

            _accentColorFile = loader.Load(this, "Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss");
            _accentFile = loader.Load(this, "Acss/Nlnet.Avalonia.Css.Fluent/Resources/Accent.acss", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources/Accent.acss");
            _themeFile = loader.Load(this, "Acss/Nlnet.Avalonia.Css.Fluent/Resources/Theme.acss", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources/Theme.acss");
            _transitions = loader.Load(this, "Acss/Nlnet.Avalonia.Css.Fluent/Resources/Transitions.acss", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources/Transitions.acss");

            loader.LoadFolder(this, "Acss/Nlnet.Avalonia.Css.Fluent/", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/");    
            loader.LoadFolder(this, "Acss/Nlnet.Avalonia.Css.Fluent/Senior", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Senior");
            loader.LoadFolder(this, "Acss/Nlnet.Avalonia.Css.Fluent/MessageBox", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/MessageBox");
        }

        public void UpdateAccent(bool reapplyStyle)
        {
            _accentFile?.Reload(reapplyStyle);
        }

        public void UpdateTransitions(bool reapplyStyle)
        {
            _transitions?.Reload(reapplyStyle);
        }

        public void UpdateTheme(bool reapplyStyle)
        {
            _themeFile?.Reload(reapplyStyle);
        }

        public void UpdateThemeColor(bool reapplyStyle)
        {
            _accentColorFile?.Reload(reapplyStyle);
        }
    }
}
