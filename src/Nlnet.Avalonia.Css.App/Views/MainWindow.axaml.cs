using System.Linq;
using Avalonia.Controls;
using Avalonia.DevTools;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.UseDevTools();

            this.DataContext = new MainWindowViewModel();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.IsLoading = false;
            }
        }

        private void MainTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            MainTabControl?
                .GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault(s => s.Name == "MainContentScrollViewer")
                ?.ScrollToHome();
        }
    }
}
