using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using System.ComponentModel;
using Avalonia.Interactivity;
using System.Diagnostics;
using System;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem( GalleryItemKind.View, "MagicPanel", null)]
    public partial class MagicPanelPage : UserControl
    {
        public MagicPanelPage()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            Panel.InvalidateMeasure();
            var random = new Random((int)DateTime.Now.Ticks);
            Canvas.SetLeft(Border1, random.NextDouble() * 200);
            Canvas.SetLeft(Border2, random.NextDouble() * 200);
            Canvas.SetLeft(Border3, random.NextDouble() * 200);
            Canvas.SetLeft(Border4, random.NextDouble() * 200);
            
            Canvas.SetTop(Border1, random.NextDouble() * 200);
            Canvas.SetTop(Border2, random.NextDouble() * 200);
            Canvas.SetTop(Border3, random.NextDouble() * 200);
            Canvas.SetTop(Border4, random.NextDouble() * 200);
        }
    }
}
