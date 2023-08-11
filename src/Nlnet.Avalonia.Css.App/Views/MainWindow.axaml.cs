using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.DevTools;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly TabControl _mainTab;

        public MainWindow()
        {
            InitializeComponent(true);

            this.DataContext = new MainWindowViewModel();

            _mainTab = this.FindControl<TabControl>("MainTabControl")!;

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

        private void MainTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (!Equals(e.Source, _mainTab))
            {
                return;
            }

            // TODO ����Ϊ�β���ֱ��ʹ��MainTabControl
            Dispatcher.UIThread.Post(() =>
            {
                _mainTab.GetVisualDescendants()
                    .OfType<ScrollViewer>()
                    .FirstOrDefault(s => s.Name == "MainContentScrollViewer")
                    ?.ScrollToHome();
            });
        }
    }
}
