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

            // Set the current mode and theme.
            CssServiceLocator.GetService<ICssManager>().Theme = "blue";
            CssServiceLocator.GetService<ICssManager>().Mode  = "light";

            // Load this.
            AvaloniaXamlLoader.Load(this);

            // Prepare type resolvers.
            var manager = CssServiceLocator.GetService<ITypeResolverManager>();
            // Nlnet.Avalonia.Css.App
            manager.LoadResolver(new GenericResolver<App>());
            // Nlnet.Avalonia.Svg
            manager.LoadResolver(new GenericResolver<Icon>());
            // Nlnet.Avalonia.Svg
            manager.LoadResolver(new GenericResolver<Case>());

            // Load application acss files.
            CssFile.Load("../../../Nlnet.Avalonia.Css.App/Css/before.loaded.acss", true);
            Dispatcher.UIThread.Post(() =>
            {
                CssFile.Load("../../../Nlnet.Avalonia.Css.App/Css/after.loaded.acss", true);
            });
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
