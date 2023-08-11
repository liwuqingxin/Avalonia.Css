using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Nlnet.Avalonia.Css.App
{
    public partial class App : Application
    {
        public IAcssFile? BeforeLoadedCssFile;
        public IAcssFile? AfterLoadedCssFile;

        public override void Initialize()
        {
            // Referenced libraries.
            AppLoader.Load("Nlnet.Avalonia.Svg.dll");
            AppLoader.Load("Avalonia.DevTools.dll");

            // Use default css builder. It has same effect to CssExtension.UseAvaloniaCssDefaultBuilder().
            AcssBuilder.UseDefaultBuilder();

            // Set the current mode and theme.
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
                const string debugRelative = "../../../Nlnet.Avalonia.Css.App/";

                BeforeLoadedCssFile = loader.Load(Application.Current.Styles, "Acss/before.loaded.acss", debugRelative);
                Dispatcher.UIThread.Post(() =>
                {
                    AfterLoadedCssFile = loader.Load(Application.Current.Styles, "Acss/after.loaded.acss", debugRelative);
                });    
            }
        }

        public void LoadBeforeLoadedCssFile()
        {
            if (Application.Current == null)
            {
                return;
            }

            var loader = AcssBuilder.Default.BuildLoader();
            const string debugRelative = "../../../Nlnet.Avalonia.Css.App/";
            BeforeLoadedCssFile = loader.Load(Application.Current.Styles, "Acss/before.loaded.acss", debugRelative);
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
