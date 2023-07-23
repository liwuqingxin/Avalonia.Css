using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Nlnet.Avalonia.SampleAssistant;
using Nlnet.Avalonia.Svg.Controls;

namespace Nlnet.Avalonia.Css.App
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            // Referenced libraries.
            AppLoader.Load("Nlnet.Avalonia.Svg.dll");
            AppLoader.Load("Avalonia.DevTools.dll");

            // Use default css builder. It has same effect to CssExtension.UseAvaloniaCssDefaultBuilder().
            CssBuilder.UseDefaultBuilder();

            // Set the current mode and theme.
            CssBuilder.Default.Configuration.Theme = "blue";
            CssBuilder.Default.Configuration.Mode  = "light";

            // Nlnet.Avalonia.Css.App
            CssBuilder.Default.LoadResolver(new GenericResolver<App>());
            // Nlnet.Avalonia.Svg
            CssBuilder.Default.LoadResolver(new GenericResolver<Icon>());
            // Nlnet.Avalonia.SampleAssistant
            CssBuilder.Default.LoadResolver(new GenericResolver<Case>());

            // Load this. In this, CssFluentTheme will be loaded.
            AvaloniaXamlLoader.Load(this);

            if (Application.Current != null)
            {
                // Load application acss files.
                var loader = CssBuilder.Default.BuildLoader();
                const string debugRelative = "../../../Nlnet.Avalonia.Css.App/";
                loader.Load(Application.Current.Styles, "Css/before.loaded.acss", debugRelative);
                Dispatcher.UIThread.Post(() =>
                {
                    loader.Load(Application.Current.Styles, "Css/after.loaded.acss", debugRelative);
                });    
            }
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
