using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.UseDevTools();

            this.DataContext = new MainWindowViewModel();
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
            // TODO 这里为何不能直接使用MainTabControl
            this.FindControl<TabControl>("MainTabControl")
                ?.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault(s => s.Name == "MainContentScrollViewer")
                ?.ScrollToHome();
        }
    }
}
