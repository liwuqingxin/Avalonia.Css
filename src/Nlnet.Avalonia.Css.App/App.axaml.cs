using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Nlnet.Sharp.Utils;

namespace Nlnet.Avalonia.Css.App
{
    public partial class App : Application
    {
        public IAcssFile? AppCssFile;

        public override void Initialize()
        {
            // Referenced libraries.
            AppLoader.Load("Nlnet.Avalonia.Svg.dll");

            // Use default css builder. It has same effect to CssExtension.UseAvaloniaCssDefaultBuilder().
            AcssBuilder.UseDefaultBuilder();

            // Set the current theme.
            AcssBuilder.Default.Configuration.Theme = "green";

            // Type resolver for Nlnet.Avalonia.Css.App
            //CssBuilder.Default.LoadResolver(new GenericResolver<App>());

            // Type resolver for Nlnet.Avalonia.Svg
            //CssBuilder.Default.LoadResolver(new GenericResolver<Icon>());

            // Type resolver for Nlnet.Avalonia.SampleAssistant
            //CssBuilder.Default.LoadResolver(new GenericResolver<Case>());

            // Load this. In this, CssFluentTheme will be loaded.
            AvaloniaXamlLoader.Load(this);

            // Build the rider settings file.
            AcssBuilder.Default.TryBuildRiderSettingsForAcss(out _, out _, null);

            if (Application.Current != null)
            {
                // Load application acss files.
                var loader = AcssBuilder.Default.BuildLoader();
                const string debugRelative = "../../src/Nlnet.Avalonia.Css.App/";

                loader.Load(Application.Current.Styles, "Acss/Case.acss", $"{debugRelative}Acss/Case.acss", true);
                loader.Load(Application.Current.Styles, "Acss/CodeEditor.acss", $"{debugRelative}Acss/CodeEditor.acss", true);
                
                AppCssFile = loader.Load(Application.Current.Styles, "Acss/app.acss", $"{debugRelative}Acss/App.acss", true);
                loader.Load(Application.Current.Styles, "Acss/pages.acss", $"{debugRelative}Acss/Pages.acss", true);
            }
        }

        public void LoadAppCssFile()
        {
            if (Application.Current == null)
            {
                return;
            }

            var loader = AcssBuilder.Default.BuildLoader();
            const string debugRelative = "../../src/Nlnet.Avalonia.Css.App/";
            AppCssFile = loader.Load(Application.Current.Styles, "Acss/app.acss", debugRelative);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new Views.MainWindow
                {
                    
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
