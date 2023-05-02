using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonLoadCss_OnClick(object? sender, RoutedEventArgs e)
        {
            var parser  = new EfficientCssParser();
            var cssFile = File.ReadAllText("./Assets/avalonia.controls.css");
            var cssStyles  = parser.TryGetStyles(cssFile).ToList();

            foreach (var cssStyle in cssStyles)
            {
                var style = (cssStyle.ToAvaloniaStyle() as Style);
                if (style?.Selector != null)
                {
                    Application.Current?.Styles.Add(style);
                }
            }
        }
    }
}
