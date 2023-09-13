using Avalonia;
using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Platform;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Nlnet.Avalonia.Controls;
using Nlnet.Avalonia.Senior.Controls;

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

        private void BtnShowDialog_OnClick(object? sender, RoutedEventArgs e)
        {
            if (this.VisualRoot is not Window owner)
            {
                MessageBox.Show("Can not find a owner to show dialog.");
                return;
            }

            var dialog = new NtWindow()
            {
                Topmost = owner.Topmost,
                Width = 600,
                Height = 500,
                Content = new WelcomePage(),
                Padding = new Thickness(24),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            dialog.ShowDialogSync(owner);
            _notificationManager?.Show("Dialog closed.");
        }
    }
}
