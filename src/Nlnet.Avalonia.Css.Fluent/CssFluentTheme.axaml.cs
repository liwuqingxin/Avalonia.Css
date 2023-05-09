using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class CssFluentTheme : Styles
    {
        public CssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);

            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Global.acss",   true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Button.acss",   true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/CheckBox.acss", true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/ComboBox.acss", true);
            CssStyles.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/TextBox.acss",  true);
        }
    }
}
