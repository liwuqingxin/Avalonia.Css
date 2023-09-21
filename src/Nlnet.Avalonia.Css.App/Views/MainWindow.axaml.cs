using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Nlnet.Avalonia.DevTools;
using Nlnet.Avalonia.Senior.Controls;
using SelectionChangedEventArgs = Avalonia.Controls.SelectionChangedEventArgs;

namespace Nlnet.Avalonia.Css.App.Views
{
    public interface IMainViewService
    {
        public void ScrollToHome();
    }
    
    public partial class MainWindow : NtWindow, IMainViewService
    {
        private readonly NtScrollViewer? _mainContentScrollViewer;

        public MainWindow()
        {
            InitializeComponent(true);

            DataContext = new MainWindowViewModel(this);
            _mainContentScrollViewer = this.FindControl<NtScrollViewer>("MainContentScrollViewer")!;

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
                _mainContentScrollViewer?.ScrollToHome();
            });
        }
    }
}
