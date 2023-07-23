using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Styling;
using Avalonia.Threading;

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
        /// <param name="owner"></param>
        /// <param name="filePath"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        public static CssFile Load(ICssBuilder cssBuilder, Styles owner, string filePath, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(cssBuilder, owner, filePath, autoLoadWhenFileChanged);
            styleFile.Load(owner, false);
            return styleFile;
        }

        /// <summary>
        /// Load a avalonia css style from an acss file asynchronously.
        /// </summary>
        /// <param name="cssBuilder"></param>
        /// <param name="owner"></param>
        /// <param name="file"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        public static CssFile BeginLoad(ICssBuilder cssBuilder, Styles owner, string file, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(cssBuilder, owner, file, autoLoadWhenFileChanged);
            styleFile.BeginLoad(owner, false);
            return styleFile;
        }

        private static CssFile CreateStyles(ICssBuilder cssBuilder, Styles owner, string filePath, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(CssFile)}.{nameof(CreateStyles)}() should be called in ui thread.");
            }

            if (owner.OfType<CssFile>().FirstOrDefault(s => s.StandardFilePath == filePath) is { } exist)
            {
                return exist;
            }

            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException($"Can not find the css file '{filePath}'.");
            }

            return new CssFile(cssBuilder, owner, filePath, autoLoadWhenFileChanged);
        }

        #endregion



        private readonly ICssBuilder             _cssBuilder;
        private readonly Styles                  _owner;
        private readonly FileSystemWatcher?      _watcher;
        private          CompositeDisposable?    _disposable;
        private          IEnumerable<ICssStyle>? _cssStyles;

        private CssFile(ICssBuilder cssBuilder, Styles owner, string filePath, bool autoLoadWhenFileChanged)
        {
            _cssBuilder      = cssBuilder;
            _owner           = owner;
            StandardFilePath = Path.GetFullPath(filePath);

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


        private DateTime _lastRead = DateTime.MinValue;

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            var lastWriteTime = File.GetLastWriteTime(StandardFilePath);
            if (lastWriteTime - _lastRead > TimeSpan.FromMilliseconds(50))
            {
                //
                // Delay 20 milliseconds to avoid conflicting with vs code, or other editors.
                //
                Task.Delay(20).ContinueWith(t =>
                {
                    BeginLoad(_owner, true);
                });
                
                _lastRead = lastWriteTime;
            }
        }

        private void Load(Styles styles, bool reapplyStyle)
        {
            var index = styles.IndexOf(this);
            
            _disposable?.Dispose();
            _disposable = null;
            _disposable ??= new CompositeDisposable();

            _disposable.Add(Disposable.Create(() =>
            {
                styles.Remove(this);

                this.Clear();
                this.Resources.Clear();
                this.Resources.MergedDictionaries.Clear();

                if (_cssStyles != null)
                {
                    foreach (var cssStyle in _cssStyles)
                    {
                        cssStyle.Dispose();
                    }
                }
                _cssStyles = null;
            }));

            try
            {
                var parser              = _cssBuilder.Parser;
                var cssContent          = File.ReadAllText(StandardFilePath);
                var css                 = parser.RemoveComments(cssContent.ToCharArray());
                var sections            = parser.ParseSections(null, css).ToList();
                var cssStyles           = sections.OfType<ICssStyle>().Where(s => !s.IsThemeChild);
                var cssThemeChildStyles = sections.OfType<ICssStyle>().Where(s => s.IsThemeChild).ToList();
                var cssDictionaryList   = sections.OfType<ICssResourceDictionary>();

                _cssStyles = sections.OfType<ICssStyle>();

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
                    var style = cssThemeChildStyle.ToAvaloniaStyle();
                    if (cssThemeChildStyle.ThemeTargetType != null)
                    {
                        // TODO 检查 ThemeVariant;
                        if (styles.TryGetResource(cssThemeChildStyle.ThemeTargetType, null, out var themeResourceObject) && themeResourceObject is ControlTheme theme)
                        {
                            // The child cache holds the references of old style instances. (In avalonia 11.0.0-preview4)
                            //typeof(StyleBase)
                            //    .GetField("_childCache", BindingFlags.Instance | BindingFlags.NonPublic)
                            //    ?.SetValue(theme, null);
                            
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

                if (reapplyStyle)
                {
                    StylerHelper.ReapplyStyling(_owner.Owner);
                }
            }
            catch (Exception e)
            {
                this.WriteError(e.ToString());
            }
        }

        private void BeginLoad(Styles styles, bool reapplyStyle)
        {
            Dispatcher.UIThread.Post(() => Load(styles, reapplyStyle));
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

        public void Reload(bool reapplyStyle)
        {
            this.BeginLoad(_owner, reapplyStyle);
        }

        public void Unload(bool reapplyStyle)
        {
            this.Dispose();

            if (reapplyStyle)
            {
                StylerHelper.ReapplyStyling(_owner.Owner);
            }

            _cssBuilder.TryRemoveCssFile(this);
        }

        #endregion
    }
}