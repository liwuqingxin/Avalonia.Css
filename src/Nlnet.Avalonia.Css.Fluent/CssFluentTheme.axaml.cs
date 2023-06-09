using System.IO;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class CssFluentTheme : Styles
    {
        public CssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);
            Load();
        }

        private static void Load()
        {
            TypeResolver.Instance.LoadResolver(new GenericResolver<CssFluentTheme>());

            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Mode.acss", true);
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Theme.acss", true);
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Resources.acss", true);

            var files = Directory.GetFiles("../../../Nlnet.Avalonia.Css.Fluent/Css/");
            foreach (var file in files)
            {
                CssFile.Load(file, true);
            }
        }

        public void UpdateResource()
        {
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Resources.acss", true);
        }

        public void UpdateMode()
        {
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Mode.acss", true);
        }

        public void UpdateTheme()
        {
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Theme.acss", true);
        }
    }
}
