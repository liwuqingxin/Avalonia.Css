using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Nlnet.Avalonia.Css
{
    public class MyButton : Button, IStyleable
    {
        Type IStyleable.StyleKey => typeof(MyButton);
    }

    public class CssStyles : Styles, IDisposable
    {
        public static void Load(string file, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(CssStyles)}.{nameof(Load)}() should be called in ui thread.");
            }

            if (Application.Current != null && Application.Current.Styles.OfType<CssStyles>().Any(s => s._file == file))
            {
                return;
            }

            var styles = new CssStyles(file, autoLoadWhenFileChanged);
            styles.Load();
        }

        public static void LoadAsync(string file, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(CssStyles)}.{nameof(LoadAsync)}() should be called in ui thread.");
            }

            if (Application.Current != null && Application.Current.Styles.OfType<CssStyles>().Any(s => s._file == file))
            {
                return;
            }

            var styles = new CssStyles(file, autoLoadWhenFileChanged);
            styles.LoadAsync();
            Application.Current?.Styles.Add(styles);
        }




        private readonly string             _file;
        private readonly FileSystemWatcher? _watcher;

        private CssStyles(string file, bool autoLoadWhenFileChanged)
        {
            _file = file;

            var dir = Path.GetDirectoryName(file);
            if (autoLoadWhenFileChanged && dir != null)
            {
                _watcher                     =  new FileSystemWatcher(dir);
                _watcher.EnableRaisingEvents =  true;
                _watcher.NotifyFilter        =  NotifyFilters.LastWrite;
                _watcher.Filter              =  "*.css";
                _watcher.Changed             += OnFileChanged;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            LoadAsync();
        }

        private void Load()
        {
            Application.Current?.Styles.Remove(this);
            this.Clear();

            try
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
                style1.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Red));
                Application.Current?.Styles.Add(style1);

                var parser    = new CssParser();
                var text      = File.ReadAllText(_file);
                var cssStyles = parser.TryGetStyles(text);

                foreach (var cssStyle in cssStyles)
                {
                    var style = (cssStyle.ToAvaloniaStyle() as Style);
                    if (style?.Selector != null)
                    {
                        this.Add(style);
                    }
                }

                Application.Current?.Styles.Add(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadAsync()
        {
            Task.Delay(20).ContinueWith(t =>
            {
                Dispatcher.UIThread.Post(Load);
            });
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}
