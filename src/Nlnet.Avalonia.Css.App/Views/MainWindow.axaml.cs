using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.DevTools;
using Avalonia.Media;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.UseDevTools();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            var pageTypes = this.GetType().Assembly
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("Nlnet.Avalonia.Css.App.Views.Pages") && t.IsAssignableTo(typeof(UserControl)));

            TabControl.Items = pageTypes.Select(t => new TabItem()
            {
                Content = Activator.CreateInstance(t),
                Header  = t.Name[..^4],
            }).ToList();
        }

        private static void LoadDynamicStyle()
        {
            Selector? selector = null;
            selector = selector.OfType<TextBox>()
                .Class("Search")
                .Class(":focus-within")
                .Template()
                .OfType(typeof(Border))
                .Name("PART_BorderElement");
            var style1 = new Style
            {
                Selector = selector
            };
            style1.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Orange));
            Application.Current?.Styles.Add(style1);
        }
    }
}
