using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Nlnet.Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using Nlnet.Sharp.Avalonia;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("Button")]
    public partial class ButtonPage : UserControl
    {
        private int _index = 1;

        public ButtonPage()
        {
            InitializeComponent();
        }

        private void RepeatButton_OnClick(object? sender, RoutedEventArgs e)
        {
            this.RepeatButton.Content = $"Click And Hold : {_index++}";
        }
    }

    public static class NlnetCommands
    {
        public static ICommand ShowMessageCommand = new MiniCommand(ShowMessage);
        
        public static void ShowMessage(object? parameter)
        {
            MessageBox.Show($"你触发了一个奇怪的命令:{parameter}");
        }
    }
}
