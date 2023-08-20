using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using Avalonia.Controls.Notifications;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("MessageBox")]
    public partial class MessageBoxPage : UserControl
    {
        private WindowNotificationManager? _notificationManager;
        
        public MessageBoxPage()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            
            _notificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(this));
        }

        private void BtnShowMessageBox_OnClick(object? sender, RoutedEventArgs e)
        {
            var buttons = (Buttons)CbxButtons.SelectionBoxItem!;
            var image = (Images)CbxImage.SelectionBoxItem!;
            var result = MessageBox.Show("Hello, this is Nlnet MessageBox :)", "Welcome", buttons, image);
            
            TbxSelectedButton.Text = result.ToString();
        }

        private async void BtnShowMessageBoxAsync_OnClick(object? sender, RoutedEventArgs e)
        {
            var buttons = (Buttons)CbxButtons.SelectionBoxItem!;
            var image   = (Images)CbxImage.SelectionBoxItem!;
            var result  = await MessageBox.ShowAsync("Hello, this is Nlnet MessageBox :)", "Welcome", buttons, image);
            
            TbxSelectedButton.Text = result.ToString();
        }

        private void BtnShowFilePicker_OnClick(object? sender, RoutedEventArgs e)
        {
            var chooser = new ManagedFileChooser();
            var window = new Window
            {
                Content = chooser,
            };

            window.Show();
        }

        private void BtnShowNotification_OnClick(object? sender, RoutedEventArgs e)
        {
            var type =(NotificationType) CbxNotificationType.SelectedItem!;
            _notificationManager?.Show(new Notification(type.ToString(), "This Acss App.", type));
        }
    }
}
