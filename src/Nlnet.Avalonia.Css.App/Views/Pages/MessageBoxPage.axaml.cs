using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using System.ComponentModel;
using Avalonia.Interactivity;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("MessageBox")]
    public partial class MessageBoxPage : UserControl
    {
        public MessageBoxPage()
        {
            InitializeComponent();
        }

        private void BtnShowMessageBox_OnClick(object? sender, RoutedEventArgs e)
        {
            var buttons = (Buttons)CbxButtons.SelectionBoxItem!;
            var image = (Images)CbxImage.SelectionBoxItem!;
            var result = MessageBox.Show("Hello, this is Nlnet MessageBox :)", "Welcome", buttons, image);
            TbxSelectedButton.Text = result.ToString();
        }
    }
}
