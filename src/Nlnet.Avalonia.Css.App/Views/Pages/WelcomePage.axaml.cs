using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("About Avalonia Css", Kind = GalleryItemKind.Welcome)]
    public partial class WelcomePage : UserControl
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void BtnOpenRepo_OnClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Content: string url })
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
    }
}
