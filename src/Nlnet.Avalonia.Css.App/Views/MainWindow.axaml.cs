using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Nlnet.Avalonia.DevTools;
using Nlnet.Avalonia.Senior.Controls;

namespace Nlnet.Avalonia.Css.App.Views
{
    public interface IMainViewService
    {
        public void ScrollToHome();
    }
    
    public partial class MainWindow : NtWindow, IMainViewService
    {
        public MainWindow()
        {
            InitializeComponent(true);

            DataContext = new MainWindowViewModel(this);

            this.UseDevTools();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.IsLoading = false;
            }
        }

        void IMainViewService.ScrollToHome()
        {
            Dispatcher.UIThread.Post(() =>
            {
                MainContentScrollViewer?.ScrollToHome();
            });
        }

        private void WelcomeHost_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
        }
    }
}
