using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using System.ComponentModel;
using Avalonia.Interactivity;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("Carousel")]
    public partial class CarouselPage : UserControl
    {
        public CarouselPage()
        {
            InitializeComponent();
        }

        private void ButtonLeft_OnClick(object? sender, RoutedEventArgs e)
        {
            Carousel.Previous();
        }

        private void ButtonRight_OnClick(object? sender, RoutedEventArgs e)
        {
            Carousel.Next();
        }
    }
}
