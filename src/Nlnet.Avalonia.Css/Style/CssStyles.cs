using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    public sealed class CssStyles : Styles, IDisposable
    {
        public static void Load(string file, bool autoLoadWhenFileChanged)
        {
            var styles = CreateStyles(file, autoLoadWhenFileChanged);
            styles?.Load();
        }

        public static void BeginLoad(string file, bool autoLoadWhenFileChanged)
        {
            var styles = CreateStyles(file, autoLoadWhenFileChanged);
            styles?.BeginLoad();
        }

        private static CssStyles? CreateStyles(string file, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(CssStyles)}.{nameof(CreateStyles)}() should be called in ui thread.");
            }

            if (Application.Current != null && Application.Current.Styles.OfType<CssStyles>().Any(s => s._file == file))
            {
                return null;
            }

            var styles = new CssStyles(file, autoLoadWhenFileChanged);
            return styles;
        }



        private readonly string _file;
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

            BeginLoad();
        }

        private void Load()
        {
            Application.Current?.Styles.Remove(this);
            this.Clear();

            try
            {
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
                switch (Application.Current?.ApplicationLifetime)
                {
                    case ClassicDesktopStyleApplicationLifetime lifetime:
                    {
                        foreach (var window in lifetime.Windows)
                        {
                            ForceApplyStyling(window);
                        }
                        break;
                    }
                    case ISingleViewApplicationLifetime {MainView: { }} singleView:
                        ForceApplyStyling(singleView.MainView);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BeginLoad()
        {
            Task.Delay(20).ContinueWith(t =>
            {
                Dispatcher.UIThread.Post(Load);
            });
        }

        private static void ForceApplyStyling(StyledElement styledElement)
        {
            try
            {
                styledElement.BeginBatchUpdate();
                AvaloniaLocator.Current.GetService<IStyler>()?.ApplyStyles(styledElement);

                if (styledElement is not IVisual visual)
                {
                    return;
                }

                foreach (var child in visual.GetVisualChildren().OfType<StyledElement>())
                {
                    ForceApplyStyling(child);
                }
            }
            finally
            {
                styledElement.EndBatchUpdate();
            }
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}
