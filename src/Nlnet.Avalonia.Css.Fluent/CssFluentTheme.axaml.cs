using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Fluent
{
    public partial class CssFluentTheme : Styles
    {
        public CssFluentTheme()
        {
            AvaloniaXamlLoader.Load(this);

            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Global.acss",  true);
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Resources.acss", true);


            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/Button.acss",   true);
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/CheckBox.acss", true);
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/ComboBox.acss", true);
            CssFile.Load("../../../Nlnet.Avalonia.Css.Fluent/Css/TextBox.acss",  true);
        }
    }
}
