using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Nlnet.Avalonia.Css.App.Utils;
using Nlnet.Avalonia.Css.App.Views;

namespace Nlnet.Avalonia.Css.App
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AppLoader.Load("Nlnet.Avalonia.Svg.dll");
            AppLoader.Load("Avalonia.DevTools.dll");

            ServiceLocator.GetService<ICssManager>().Theme = "blue";
            ServiceLocator.GetService<ICssManager>().Mode  = "light";

            AvaloniaXamlLoader.Load(this);

            var manager = ServiceLocator.GetService<ITypeResolverManager>();
            manager.LoadResolver(new GenericResolver<App>());

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
                desktop.MainWindow = new MainWindow
                {
                    
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
