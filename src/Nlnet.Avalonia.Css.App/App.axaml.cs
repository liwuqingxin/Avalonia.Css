using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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

            CssManager.Current.Theme = "blue";
            CssManager.Current.Mode  = "light";

            AvaloniaXamlLoader.Load(this);
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
