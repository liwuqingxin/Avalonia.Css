using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Nlnet.Avalonia.Css.Sample.LocalContext.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(true);
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
