using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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



        private readonly IAcssBuilder _acssBuilder;
        private readonly Styles _owner;
        private CompositeDisposable? _disposable;
        private AcssTokens? _tokens;

        private AcssFile(IAcssBuilder acssBuilder, Styles owner, string standardFilePath, bool autoLoadWhenFileChanged)
        {
            _acssBuilder     = acssBuilder;
            _owner           = owner;
            StandardFilePath = standardFilePath;
        }

        private void Load(Styles styles, bool reapplyStyle)
        {
            var index = styles.IndexOf(this);
            
            _disposable?.Dispose();
            _disposable = null;
            _disposable ??= new CompositeDisposable();

            try
            {
                _tokens = AcssTokens.Get(_acssBuilder, StandardFilePath);
                _tokens.FileChanged -= TokensOnFileChanged;
                _tokens.FileChanged += TokensOnFileChanged;

                var acssNormalStyles = _tokens.GetNormalStyles().ToList();
                var acssThemeChildStyles = _tokens.GetThemeStyles().ToList();
                var acssDictionaryList = _tokens.GetResourceDictionaries().ToList();

                // Normal styles.
                foreach (var acssStyle in acssNormalStyles)
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
                    _tokens.FileChanged -= TokensOnFileChanged;

                    _owner.Owner.DetachStylesRecursively(this.OfType<Style>().ToList());
                    styles.Remove(this);

                    this.Clear();
                    this.Resources.Clear();
                    this.Resources.ThemeDictionaries.Clear();
                    this.Resources.MergedDictionaries.Clear();

                    foreach (var acssStyle in _tokens.GetStyles())
                    {
                        acssStyle.Dispose();
                    }
                }));

                if (reapplyStyle)
                {
                    var normalTypes = acssNormalStyles.Select(s => s.GetTargetType()).Where(t => t != null).ToList();
                    _owner.Owner.ReapplyStyling(false, normalTypes!);

                    var themeTypes = acssThemeChildStyles.Select(s => s.GetTargetType()).Where(t => t != null).ToList();
                    _owner.Owner.ReapplyStyling(true, themeTypes!);
                }
            }
            catch (Exception e)
            {
                this.WriteError(e.ToString());
            }
        }

        private void TokensOnFileChanged(object? sender, EventArgs e)
        {
            BeginLoad(_owner, true);
        }

        private void BeginLoad(Styles styles, bool reapplyStyle)
        {
            Dispatcher.UIThread.Post(() => Load(styles, reapplyStyle));
        }

        public void Dispose()
        {
            _disposable?.Dispose();
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

        public void Unload()
        {
            this.Dispose();

            _acssBuilder.TryRemoveAcssFile(this);
        }

        #endregion
    }
}