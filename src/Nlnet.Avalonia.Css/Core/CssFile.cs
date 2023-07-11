using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// A css style instance that associated to a .acss file.
    /// </summary>
    internal sealed class CssFile : Styles, ICssFile, IDisposable
    {
        #region Static

        /// <summary>
        /// Load a avalonia css style from an acss file synchronously.
        /// </summary>
        /// <param name="cssBuilder"></param>
        /// <param name="styles"></param>
        /// <param name="filePath"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        public static CssFile Load(ICssBuilder cssBuilder, Styles styles, string filePath, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(cssBuilder, styles, filePath, autoLoadWhenFileChanged);
            styleFile.Load(styles);
            return styleFile;
        }

        /// <summary>
        /// Load a avalonia css style from an acss file asynchronously.
        /// </summary>
        /// <param name="cssBuilder"></param>
        /// <param name="styles"></param>
        /// <param name="file"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        public static CssFile BeginLoad(ICssBuilder cssBuilder, Styles styles, string file, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(cssBuilder, styles, file, autoLoadWhenFileChanged);
            styleFile.BeginLoad(styles);
            return styleFile;
        }

        private static CssFile CreateStyles(ICssBuilder cssBuilder, Styles styles, string filePath, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(CssFile)}.{nameof(CreateStyles)}() should be called in ui thread.");
            }

            if (styles.OfType<CssFile>().FirstOrDefault(s => s.StandardFilePath == filePath) is { } exist)
            {
                return exist;
            }

            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException($"Can not find the css file '{filePath}'.");
            }

            return new CssFile(cssBuilder, styles, filePath, autoLoadWhenFileChanged);
        }

        #endregion



        private readonly ICssBuilder          _cssBuilder;
        private readonly Styles               _styles;
        private readonly FileSystemWatcher?   _watcher;
        private          CompositeDisposable? _disposable;

        private CssFile(ICssBuilder cssBuilder, Styles styles, string filePath, bool autoLoadWhenFileChanged)
        {
            _cssBuilder = cssBuilder;
            _styles     = styles;
            StandardFilePath       = Path.GetFullPath(filePath);

            var dir = Path.GetDirectoryName(filePath);
            if (autoLoadWhenFileChanged && dir != null)
            {
                // TODO 改为文件监控，而不是文件夹监控
                // TODO 统一处理监控，支持文件夹统一监控，而不用多实例监控
                _watcher                     =  new FileSystemWatcher(dir);
                _watcher.EnableRaisingEvents =  true;
                _watcher.NotifyFilter        =  NotifyFilters.LastWrite;
                _watcher.Filter              =  $"{Path.GetFileName(filePath)}";
                _watcher.Changed             += OnFileChanged;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            BeginLoad(_styles);
        }

        private void Load(Styles styles)
        {
            var index = styles.IndexOf(this);
            styles.Remove(this);

            _disposable?.Dispose();
            _disposable = null;

            this.Clear();
            this.Resources.Clear();
            this.Resources.MergedDictionaries.Clear();

            try
            {
                var parser              = _cssBuilder.Parser;
                var cssContent          = File.ReadAllText(StandardFilePath);
                var css                 = parser.RemoveComments(cssContent.ToCharArray());
                var sections            = parser.ParseSections(null, css).ToList();
                var cssStyles           = sections.OfType<ICssStyle>().Where(s => !s.IsThemeChild);
                var cssThemeChildStyles = sections.OfType<ICssStyle>().Where(s => s.IsThemeChild).ToList();
                var cssDictionaryList   = sections.OfType<ICssResourceDictionary>();

                foreach (var cssStyle in cssStyles)
                {
                    var style = cssStyle.ToAvaloniaStyle();
                    if (style.Selector != null)
                    {
                        this.Add(style);
                    }
                }

                foreach (var cssThemeChildStyle in cssThemeChildStyles)
                {
                    _disposable ??= new CompositeDisposable(cssThemeChildStyles.Count);
                    var style = cssThemeChildStyle.ToAvaloniaStyle();
                    if (cssThemeChildStyle.TargetType != null)
                    {
                        if (styles.TryGetResource(cssThemeChildStyle.TargetType, out var themeResourceObject) && themeResourceObject is ControlTheme theme)
                        {
                            // The child cache holds the references of old style instances.
                            typeof(StyleBase)
                                .GetField("_childCache", BindingFlags.Instance | BindingFlags.NonPublic)
                                ?.SetValue(theme, null);
                            
                            //
                            // TODO Do not consider the older of old and new styles now.
                            //
                            theme.Add(style);

                            _disposable.Add(Disposable.Create(() =>
                            {
                                theme.Children.Remove(style);
                            }));
                        }
                    }
                }

                foreach (var dictionary in cssDictionaryList)
                {
                    var dic = dictionary.ToAvaloniaResourceDictionary(_cssBuilder);
                    if (dic != null)
                    {
                        this.Resources.MergedDictionaries.Add(dic);
                    }
                }

                if (index == -1)
                {
                    styles.Add(this);
                }
                else
                {
                    styles.Insert(index, this);
                }

                ReapplyStyling();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BeginLoad(Styles styles)
        {
            Task.Delay(20).ContinueWith(t =>
            {
                Dispatcher.UIThread.Post(() => Load(styles));
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
            _disposable?.Dispose();
            _watcher?.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(CssFile)} {StandardFilePath}";
        }



        #region ICssFile

        public string StandardFilePath { get; }

        public void Reload()
        {
            this.Load(_styles);
        }

        #endregion
    }
}
