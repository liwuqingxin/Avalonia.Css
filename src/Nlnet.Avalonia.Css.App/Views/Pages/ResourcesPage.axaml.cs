using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using System.ComponentModel;
using Avalonia.Interactivity;
using System.Diagnostics;
using System;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem( GalleryItemKind.View, "Resources", null)]
    public partial class ResourcesPage : UserControl
    {
        public ResourcesPage()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.douyin.com/video/7269771109397056768") { UseShellExecute = true });
        }
    }
}
