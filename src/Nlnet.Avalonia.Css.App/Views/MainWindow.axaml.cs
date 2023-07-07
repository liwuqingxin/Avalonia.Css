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

            //var pageTypes = this.GetType().Assembly
            //    .GetTypes()
            //    .Where(t => t.Namespace != null && t.Namespace.StartsWith("Nlnet.Avalonia.Css.App.Views.Pages")
            //        && t.IsAssignableTo(typeof(UserControl)));

            //var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
            //TabControl.Items = pageTypes.Select(t =>
            //{
            //    var tabItem = new TabItem()
            //    {
            //        Content = Activator.CreateInstance(t),
            //        Header  = t.Name[..^4],
            //    };

            //    if (assetLoader != null)
            //    {
            //        var imageUriString = $"avares://Nlnet.Avalonia.Css.App/Assets/Svg/{t.Name[..^4]}.svg";
            //        if (assetLoader.Exists(new Uri(imageUriString)) == false)
            //        {
            //            imageUriString = $"avares://Nlnet.Avalonia.Css.App/Assets/Svg/default.svg";
            //        }
            //        var svg = new Icon()
            //        {
            //            Margin = new Thickness(12, 0, 0, 0),
            //            Width  = 28,
            //            Height = 28,
            //        };
            //        RenderOptions.SetBitmapInterpolationMode(svg, BitmapInterpolationMode.HighQuality);
            //        LoadedStateMixin.SetUseLoadedState(svg, true);
            //        Nlnet.Avalonia.Svg.Controls.Icon.SetIconSize(svg, 28);
            //        Nlnet.Avalonia.Svg.Controls.Icon.SetIconSvg(svg, new Uri(imageUriString));
            //        TabItemExtension.SetIconContent(tabItem, svg);
            //    }

            //    return tabItem;
            //}).ToList();

            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.IsLoading = false;
            }
        }
    }
}
