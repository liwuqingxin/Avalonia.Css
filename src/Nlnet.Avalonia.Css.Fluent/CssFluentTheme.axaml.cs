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
    }
}
