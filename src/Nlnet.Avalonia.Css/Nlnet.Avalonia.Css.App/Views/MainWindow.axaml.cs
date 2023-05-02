using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Nlnet.Avalonia.Css.App.Views
{
    public partial class MainWindow : Window
    {
        private static FileSystemWatcher? _watcher;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private static void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            _watcher                     =  new FileSystemWatcher("../../Assets");
            _watcher.EnableRaisingEvents =  true;
            _watcher.NotifyFilter        =  NotifyFilters.LastWrite;
            _watcher.Filter              =  "*.css";
            _watcher.Changed             += WatcherOnChanged;
        }

        private static void MainWindow_Unloaded(object? sender, RoutedEventArgs e)
        {
            _watcher?.Dispose();
        }

        private static void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            ReloadCss();
        }

        private void ButtonLoadCss_OnClick(object? sender, RoutedEventArgs e)
        {
            ReloadCss();
        }

        private static void ReloadCss()
        {
            Task.Delay(50).ContinueWith(t =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    try
                    {
                        var parser    = new CssParser();
                        var text      = File.ReadAllText("../../Assets/avalonia.controls.css");
                        var cssStyles = parser.TryGetStyles(text);

                        foreach (var cssStyle in cssStyles)
                        {
                            var style = (cssStyle.ToAvaloniaStyle() as Style);
                            if (style?.Selector != null)
                            {
                                Application.Current?.Styles.Add(style);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            });
        }
    }
}
