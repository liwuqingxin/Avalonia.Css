using Avalonia.Controls;
using Avalonia.Interactivity;
using Nlnet.Avalonia.DevTools;

namespace Nlnet.Avalonia.Css.Sample.LocalContext.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(true);

            this.UseDevTools();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            new AcssWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            }.Show();
        }
    }
}
