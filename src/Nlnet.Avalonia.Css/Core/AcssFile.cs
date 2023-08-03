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
    /// An acss style instance that associated to a .acss file.
    /// </summary>
    internal sealed class AcssFile : Styles, IAcssFile, IDisposable
    {
        #region Static

        /// <summary>
        /// Load a avalonia css style from an acss file synchronously.
        /// </summary>
        /// <param name="acssBuilder"></param>
        /// <param name="owner"></param>
        /// <param name="filePath"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        public static AcssFile Load(IAcssBuilder acssBuilder, Styles owner, string filePath, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(acssBuilder, owner, filePath, autoLoadWhenFileChanged);
            styleFile.Load(owner, false);
            return styleFile;
        }

        /// <summary>
        /// Load a avalonia css style from an acss file asynchronously.
        /// </summary>
        /// <param name="acssBuilder"></param>
        /// <param name="owner"></param>
        /// <param name="file"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        public static AcssFile BeginLoad(IAcssBuilder acssBuilder, Styles owner, string file, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(acssBuilder, owner, file, autoLoadWhenFileChanged);
            styleFile.BeginLoad(owner, false);
            return styleFile;
        }

        private static AcssFile CreateStyles(IAcssBuilder acssBuilder, Styles owner, string filePath, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(AcssFile)}.{nameof(CreateStyles)}() should be called in ui thread.");
            }

            if (owner.OfType<AcssFile>().FirstOrDefault(s => s.StandardFilePath == filePath) is { } exist)
            {
                return exist;
            }

            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException($"Can not find the acss file '{filePath}'.");
            }

            return new AcssFile(acssBuilder, owner, filePath, autoLoadWhenFileChanged);
        }

        #endregion



        private readonly IAcssBuilder             _acssBuilder;
        private readonly Styles                  _owner;
        private readonly FileSystemWatcher?      _watcher;
        private          CompositeDisposable?    _disposable;
        private          IEnumerable<IAcssStyle>? _acssStyles;

        private AcssFile(IAcssBuilder acssBuilder, Styles owner, string filePath, bool autoLoadWhenFileChanged)
        {
            _acssBuilder      = acssBuilder;
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
                this.Resources.ThemeDictionaries.Clear();
                this.Resources.MergedDictionaries.Clear();

                if (_acssStyles != null)
                {
                    foreach (var acssStyle in _acssStyles)
                    {
                        acssStyle.Dispose();
                    }
                }
                _acssStyles = null;
            }));

            try
            {
                var parser = _acssBuilder.Parser;
                var acssContent = File.ReadAllText(StandardFilePath);
                var acss = parser.RemoveCommentsAndLineBreaks(acssContent.ToCharArray());
                var sections = parser.ParseSections(null, acss).ToList();
                var acssStyles = sections.OfType<IAcssStyle>().Where(s => !s.IsThemeChild);
                var acssThemeChildStyles = sections.OfType<IAcssStyle>().Where(s => s.IsThemeChild).ToList();
                var acssDictionaryList = sections.OfType<IAcssResourceDictionary>();

                _acssStyles = sections.OfType<IAcssStyle>();

                foreach (var acssStyle in acssStyles)
                {
                    var style = acssStyle.ToAvaloniaStyle();
                    if (style.Selector != null)
                    {
                        this.Add(style);
                    }
                }

                foreach (var acssThemeChildStyle in acssThemeChildStyles)
                {
                    var style = acssThemeChildStyle.ToAvaloniaStyle();
                    if (acssThemeChildStyle.ThemeTargetType != null)
                    {
                        // TODO 检查 ThemeVariant;
                        if (styles.TryGetResource(acssThemeChildStyle.ThemeTargetType, null, out var themeResourceObject) && themeResourceObject is ControlTheme theme)
                        {
                            //
                            // TODO Do not consider the older of old and new styles now.
                            //
                            theme.Add(style);

                            _disposable.Add(Disposable.Create(() =>
                            {
                                theme.Children.Remove(style);
                            }));
                        }
                        else
                        {

                        }
                    }
                }

                foreach (var dictionary in acssDictionaryList)
                {
                    var dic = dictionary.ToAvaloniaResourceDictionary(_acssBuilder);
                    if (dic != null)
                    {
                        if (dictionary.IsModeResource())
                        {
                            this.Resources.ThemeDictionaries.Add(dictionary.GetThemeVariant(), dic);
                        }
                        else
                        {
                            this.Resources.MergedDictionaries.Add(dic);
                        }
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
            return $"{nameof(AcssFile)} {StandardFilePath}";
        }



        #region IAcssFile

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

            _acssBuilder.TryRemoveAcssFile(this);
        }

        #endregion
    }
}