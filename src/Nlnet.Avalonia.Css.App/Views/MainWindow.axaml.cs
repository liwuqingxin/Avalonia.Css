using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly TabControl _mainTab;

        public MainWindow()
        {
            InitializeComponent();

            //this.UseDevTools();

            this.DataContext = new MainWindowViewModel();

            _mainTab = this.FindControl<TabControl>("MainTabControl")!;
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

            // TODO 这里为何不能直接使用MainTabControl
            _mainTab.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault(s => s.Name == "MainContentScrollViewer")
                ?.ScrollToHome();
        }
    }
}
