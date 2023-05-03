using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        private void BtnLoadCss_OnClick(object? sender, RoutedEventArgs e)
        {
            CssStyles.Load("../../Assets/avalonia.controls.css", true);
        }

        private void BtnClearTheme_OnClick(object? sender, RoutedEventArgs e)
        {
            Application.Current!.Styles.Clear();
        }

        private void BtnAddTheme_OnClick(object? sender, RoutedEventArgs e)
        {
            Application.Current!.Styles.Add(new FluentTheme(new Uri("avares://Nlnet.Avalonia.Css/", UriKind.Absolute)));
        }
    }
}