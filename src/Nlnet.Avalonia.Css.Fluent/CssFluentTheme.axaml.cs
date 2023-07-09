using System.IO;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class CssFluentTheme : Styles
    {
        static CssFluentTheme()
        {
            TemplatedControlExtension.Init();
        }

        public CssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);
            Load();
        }

        private void Load()
        {
            var typeResolverManager = CssServiceLocator.GetService<ITypeResolverManager>();
            typeResolverManager.LoadResolver(new GenericResolver<CssFluentTheme>());

            this.LoadCssFile("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Mode.acss");
            this.LoadCssFile("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Theme.acss");
            this.LoadCssFile("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Resources.acss");

            var files = Directory.GetFiles("../../../Nlnet.Avalonia.Css.Fluent/Css/");
            foreach (var file in files)
            {
                this.LoadCssFile(file);
            }
        }

        public void UpdateResource()
        {
            this.LoadCssFile("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Resources.acss");
        }

        public void UpdateMode()
        {
            this.LoadCssFile("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Mode.acss");
        }

        public void UpdateTheme()
        {
            this.LoadCssFile("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Theme.acss");
        }
    }
}
