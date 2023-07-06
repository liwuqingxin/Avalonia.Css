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
    public sealed class CssFile : Styles, IDisposable
    {
        public static void Load(string filePath, bool autoLoadWhenFileChanged)
        {
            var styles = CreateStyles(filePath, autoLoadWhenFileChanged);
            styles?.Load();
        }

        public static void BeginLoad(string file, bool autoLoadWhenFileChanged)
        {
            var styles = CreateStyles(file, autoLoadWhenFileChanged);
            styles?.BeginLoad();
        }

        private static CssFile? CreateStyles(string filePath, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(CssFile)}.{nameof(CreateStyles)}() should be called in ui thread.");
            }

            if (Application.Current?.Styles.OfType<CssFile>().FirstOrDefault(s => s._file == filePath) is { } exist)
            {
                return exist;
            }

            return new CssFile(filePath, autoLoadWhenFileChanged);
        }



        private readonly string _file;
        private readonly FileSystemWatcher? _watcher;

        private CssFile(string filePath, bool autoLoadWhenFileChanged)
        {
            _file = filePath;

            var dir = Path.GetDirectoryName(filePath);
            if (autoLoadWhenFileChanged && dir != null)
            {
                _watcher                     =  new FileSystemWatcher(dir);
                _watcher.EnableRaisingEvents =  true;
                _watcher.NotifyFilter        =  NotifyFilters.LastWrite;
                _watcher.Filter              =  $"{Path.GetFileName(filePath)}";
                _watcher.Changed             += OnFileChanged;
            }

            //Application.Current?.Styles.Add(this);
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
            if (Application.Current == null)
            {
                throw new InvalidOperationException("Application has not been prepared. Can not load the css file.");
            }

            var index = Application.Current.Styles.IndexOf(this);
            Application.Current.Styles.Remove(this);

            this.Clear();
            this.Resources.Clear();
            this.Resources.MergedDictionaries.Clear();

            try
            {
                var parser            = CssServiceLocator.GetService<ICssParser>();
                var cssContent        = File.ReadAllText(_file);
                var css               = parser.RemoveComments(cssContent.ToCharArray());
                var sections          = parser.ParseSections(null, css).ToList();
                var cssStyles         = sections.OfType<ICssStyle>();
                var cssDictionaryList = sections.OfType<ICssResourceDictionary>();

                foreach (var cssStyle in cssStyles)
                {
                    var style = cssStyle.ToAvaloniaStyle();
                    if (style.Selector != null)
                    {
                        this.Add(style);
                    }
                }

                foreach (var dictionary in cssDictionaryList)
                {
                    var dic = dictionary.ToAvaloniaResourceDictionary();
                    if (dic != null)
                    {
                        this.Resources.MergedDictionaries.Add(dic);
                    }
                }

                if (index == -1)
                {
                    Application.Current.Styles.Add(this);
                }
                else
                {
                    Application.Current.Styles.Insert(index, this);
                }

                ReapplyStyling();
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

        private static void ReapplyStyling()
        {
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
                case ISingleViewApplicationLifetime { MainView: { } } singleView:
                    ForceApplyStyling(singleView.MainView);
                    break;
            }
        }

        private static void ForceApplyStyling(StyledElement styledElement)
        {
            ((IStyleable)styledElement).DetachStyles();

            try
            {
                //styledElement.InvalidateStyles();

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

        public override string ToString()
        {
            return $"{nameof(CssFile)} {_file}";
        }
    }
}
