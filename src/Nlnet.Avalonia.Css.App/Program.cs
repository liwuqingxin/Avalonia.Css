using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                typeof(App).WriteError(e.ToString());
                if (e.InnerException != null)
                {
                    typeof(App).WriteError(e.InnerException.ToString());
                }
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        private static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()

                // Avalonia css stuff.
                .UseAvaloniaCssDefaultBuilder()
                // Type resolver for Nlnet.Avalonia.Css.App
                .WithTypeResolverForDefaultBuilder(new GenericResolver<App>())
                // Type resolver for Nlnet.Avalonia.SampleAssistant
                .WithTypeResolverForDefaultBuilder(new GenericResolver<Case>())
                
                ;
        }
    }
}
