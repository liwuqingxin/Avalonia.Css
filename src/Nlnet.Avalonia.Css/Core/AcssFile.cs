using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
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
        /// <param name="standardFilePath"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        internal static AcssFile TryLoad(IAcssBuilder acssBuilder, Styles owner, string standardFilePath, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(acssBuilder, owner, standardFilePath, autoLoadWhenFileChanged);
            styleFile.Load(owner, false);
            return styleFile;
        }

        /// <summary>
        /// Load a avalonia css style from an acss file asynchronously.
        /// </summary>
        /// <param name="acssBuilder"></param>
        /// <param name="owner"></param>
        /// <param name="standardFilePath"></param>
        /// <param name="autoLoadWhenFileChanged"></param>
        /// <returns></returns>
        internal static AcssFile TryBeginLoad(IAcssBuilder acssBuilder, Styles owner, string standardFilePath, bool autoLoadWhenFileChanged = true)
        {
            var styleFile = CreateStyles(acssBuilder, owner, standardFilePath, autoLoadWhenFileChanged);
            styleFile.BeginLoad(owner, false);
            return styleFile;
        }

        private static AcssFile CreateStyles(IAcssBuilder acssBuilder, Styles owner, string standardFilePath, bool autoLoadWhenFileChanged)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(AcssFile)}.{nameof(CreateStyles)}() should be called in ui thread.");
            }

            if (owner.OfType<AcssFile>().FirstOrDefault(s => s.StandardFilePath == standardFilePath) is { } exist)
            {
                return exist;
            }

            if (File.Exists(standardFilePath) == false)
            {
                throw new FileNotFoundException($"Can not find the acss file '{standardFilePath}'.");
            }

            return new AcssFile(acssBuilder, owner, standardFilePath, autoLoadWhenFileChanged);
        }

        #endregion



        private readonly IAcssBuilder             _acssBuilder;
        private readonly Styles                  _owner;
        private readonly FileSystemWatcher?      _watcher;
        private          CompositeDisposable?    _disposable;
        private          IEnumerable<IAcssStyle>? _acssStyles;

        private AcssFile(IAcssBuilder acssBuilder, Styles owner, string standardFilePath, bool autoLoadWhenFileChanged)
        {
            _acssBuilder     = acssBuilder;
            _owner           = owner;
            StandardFilePath = standardFilePath;

            var dir = Path.GetDirectoryName(standardFilePath);
            if (autoLoadWhenFileChanged && dir != null)
            {
                // TODO 改为文件监控，而不是文件夹监控
                // TODO 统一处理监控，支持文件夹统一监控，而不用多实例监控
                _watcher                     =  new FileSystemWatcher(dir);
                _watcher.EnableRaisingEvents =  true;
                _watcher.NotifyFilter        =  NotifyFilters.LastWrite;
                _watcher.Filter              =  $"{Path.GetFileName(standardFilePath)}";
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

            try
            {
                var parser = _acssBuilder.Parser;
                var acssContent = File.ReadAllText(StandardFilePath);
                var acssSpan = parser.RemoveCommentsAndLineBreaks(acssContent.ToCharArray());
                var sections = parser.ParseSections(null, acssSpan).ToList();
                var acssStyles = sections.OfType<IAcssStyle>().Where(s => !s.IsThemeChild);
                var acssThemeChildStyles = sections.OfType<IAcssStyle>().Where(s => s.IsThemeChild).ToList();
                var acssDictionaryList = sections.OfType<IAcssResourceDictionary>();

                _acssStyles = sections.OfType<IAcssStyle>();

                // Normal styles.
                foreach (var acssStyle in acssStyles)
                {
                    var style = acssStyle.ToAvaloniaStyle();
                    if (style.Selector != null)
                    {
                        this.Add(style);
                    }
                }

                // Theme child styles.
                foreach (var acssThemeChildStyle in acssThemeChildStyles)
                {
                    var style = acssThemeChildStyle.ToAvaloniaStyle();
                    if (acssThemeChildStyle.ThemeTargetType == null)
                    {
                        continue;
                    }
                    
                    // TODO 检查 ThemeVariant;
                    var suc = styles.TryGetResource(acssThemeChildStyle.ThemeTargetType, null, out var themeResourceObject);
                    if (suc == false && Application.Current != null)
                    {
                        suc = Application.Current.TryGetResource(acssThemeChildStyle.ThemeTargetType, null, out themeResourceObject);
                    }
                    if (themeResourceObject is ControlTheme theme)
                    {
                        //
                        // TODO Do not consider the older of old and new styles now.
                        //
                        theme.Add(style);

                        _disposable.Add(Disposable.Create(() =>
                        {
                            _owner.Owner.DetachStylesRecursively(new List<Style>(){style});
                            theme.Children.Remove(style);
                        }));
                    }
                    else
                    {
                        this.WriteError($"Can not find the ControlTheme for '{acssThemeChildStyle.ThemeTargetType}'. Skip it.");
                    }
                }

                // Resources
                foreach (var dictionary in acssDictionaryList)
                {
                    var dic = dictionary.ToAvaloniaResourceDictionary(_acssBuilder);
                    if (dic == null)
                    {
                        continue;
                    }
                    if (dictionary.IsModeResource())
                    {
                        this.Resources.ThemeDictionaries.Add(dictionary.GetThemeVariant(), dic);
                    }
                    else
                    {
                        this.Resources.MergedDictionaries.Add(dic);
                    }
                }

                // Put this acss file to it's owner styles.
                if (index == -1)
                {
                    styles.Add(this);
                }
                else
                {
                    styles.Insert(index, this);
                }
                
                _disposable.Add(Disposable.Create(() =>
                {
                    _owner.Owner.DetachStylesRecursively(this.OfType<Style>().ToList());
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

                if (reapplyStyle)
                {
                    _owner.Owner.ReapplyStyling();
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
                _owner.Owner.ReapplyStyling();
            }

            _acssBuilder.TryRemoveAcssFile(this);
        }

        #endregion
    }
}