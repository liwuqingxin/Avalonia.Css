using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class CssFluentTheme : Styles
    {
        public CssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);

            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Global.css", true);

            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Button.css",   true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/CheckBox.css", true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/ComboBox.css", true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/TextBox.css",  true);
        }
    }
}
