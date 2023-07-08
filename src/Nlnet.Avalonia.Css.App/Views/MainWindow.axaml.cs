using Avalonia.Controls;
using Avalonia.DevTools;

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
    }
}
