using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
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
}
