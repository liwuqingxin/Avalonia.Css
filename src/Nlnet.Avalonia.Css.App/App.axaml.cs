using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Nlnet.Avalonia.SampleAssistant;
using Nlnet.Avalonia.Svg.Controls;
using Nlnet.Sharp.Utils;

namespace Nlnet.Avalonia.Css.App
{
    public static class DebugThing
    {
        public static string? PreferLocalPath { get; set; }

        static DebugThing()
        {
#if DEBUG
            PreferLocalPath = "../../src/Nlnet.Avalonia.Css.Fluent/";
#else
            PreferLocalPath = null;
#endif
        }
    }

    public partial class App : Application
    {
        public IAcssFile? AppCssFile;

        public override void Initialize()
        {
            // Referenced libraries.
            AppLoader.Load("Nlnet.Avalonia.Svg.dll");

            // Use default css builder. It has same effect to CssExtension.UseAvaloniaCssDefaultBuilder().
            AcssContext.UseDefaultContext();

            var typeResolverManager = AcssContext.Default.GetService<ITypeResolverManager>();
            var loader = AcssContext.Default.GetService<IAcssLoader>();
            var cfg = AcssContext.Default.GetService<IAcssConfiguration>();
            var riderBuilder = AcssContext.Default.GetService<IRiderSettingsBuilder>();


            // Set the current theme.
            cfg.Theme = "green";
            cfg.EnableTransitions = true;

            // Type resolver for Nlnet.Avalonia.Css.App
            typeResolverManager.LoadResolver(new GenericTypeResolver<App>());

            // Type resolver for Nlnet.Avalonia.Svg
            typeResolverManager.LoadResolver(new GenericTypeResolver<Icon>());

            // Type resolver for Nlnet.Avalonia.SampleAssistant
            typeResolverManager.LoadResolver(new GenericTypeResolver<Case>());

            // Load this. In this, CssFluentTheme will be loaded.
            AvaloniaXamlLoader.Load(this);

            // Build the rider settings file.
            riderBuilder.TryBuildRiderSettingsForAcss(out _, out _, null);

            if (Application.Current != null)
            {
                // Load application acss files.
                const string debugRelative = "../../src/Nlnet.Avalonia.Css.App/";

                loader.Load(Application.Current.Styles, new FileSource("Acss/Case.acss", $"{debugRelative}Acss/Case.acss"));
                loader.Load(Application.Current.Styles, new FileSource("Acss/CodeEditor.acss", $"{debugRelative}Acss/CodeEditor.acss"));
                loader.Load(Application.Current.Styles, new FileSource("Acss/Pages.acss", $"{debugRelative}Acss/Pages.acss"));

                LoadAppCssFile();
            }
        }

        public void LoadAppCssFile()
        {
            if (Application.Current == null)
            {
                return;
            }

            var loader = AcssContext.Default.GetService<IAcssLoader>();
            const string debugRelative = "../../src/Nlnet.Avalonia.Css.App/";
            AppCssFile = loader.Load(Application.Current.Styles, new FileSource("Acss/App.acss", $"{debugRelative}Acss/App.acss"));
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
