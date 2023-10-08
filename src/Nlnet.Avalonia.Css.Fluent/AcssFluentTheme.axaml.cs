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
            AcssContext.UseDefaultContext();

            var resProvidersManager = AcssContext.Default.GetService<IResourceProvidersManager>();
            var typeResolverManager = AcssContext.Default.GetService<ITypeResolverManager>();
            var loader = AcssContext.Default.GetService<IAcssLoader>();

            // This is not added to application's styles till now. Register this to resource manager to enable resource access to this.
            resProvidersManager.RegisterResourceProvider(this);

            // Nlnet.Avalonia.Css.Controls
            typeResolverManager.LoadResolver(new GenericTypeResolver<NotifyChangeContentPresenter>());

            // Avalonia.Controls.DataGrid
            typeResolverManager.LoadResolver(new GenericTypeResolver<DataGrid>());

            // Nlnet.Avalonia.Senior
            typeResolverManager.LoadResolver(new GenericTypeResolver<NtScrollViewer>());

            // Nlnet.Avalonia.MessageBox
            typeResolverManager.LoadResolver(new GenericTypeResolver<MessageBox>());

            const string debugRelative = "../../src/Nlnet.Avalonia.Css.Fluent/";

            _accentColorFile = loader.Load(this, new FileSource("Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss"));
            
            loader.LoadCollection(this, new FileSourceCollection("Acss/Nlnet.Avalonia.Css.Fluent/Resources", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources"));
            loader.LoadCollection(this, new FileSourceCollection("Acss/Nlnet.Avalonia.Css.Fluent/", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/"));
            loader.LoadCollection(this, new FileSourceCollection("Acss/Nlnet.Avalonia.Css.Fluent/Senior", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Senior"));
            loader.LoadCollection(this, new FileSourceCollection("Acss/Nlnet.Avalonia.Css.Fluent/MessageBox", $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/MessageBox"));
        }

        public void UpdateThemeColor(bool reapplyStyle)
        {
            _accentColorFile?.Reload(reapplyStyle);
        }
    }
}
