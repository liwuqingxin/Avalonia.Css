﻿using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// An acss style instance that associated to a .acss source.
    /// </summary>
    internal sealed class AcssFile : Styles, IAcssFile, IDisposable
    {
        #region Static

        /// <summary>
        /// Load an avalonia css style from an acss source synchronously.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">UI thread required.</exception>
        internal static AcssFile TryLoad(IAcssContext context, Styles owner, ISource source)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                throw new InvalidOperationException($"{nameof(TryLoad)}() must be called in ui thread.");
            }

            var styleFile = new AcssFile(context, owner, source);
            styleFile.Load(owner, false);

            return styleFile;
        }

        #endregion



        private readonly IAcssContext _context;
        private readonly Styles _owner;
        private CompositeDisposable? _disposable;
        private AcssTokens? _tokens;

        private AcssFile(IAcssContext context, Styles owner, ISource source)
        {
            _context = context;
            _owner   = owner;
            Source   = source;
        }

        private void Load(Styles styles, bool reapplyStyle)
        {
            var index = styles.IndexOf(this);
            
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            try
            {
                _tokens = AcssTokens.Get(_context, Source);
                if (_tokens == null)
                {
                    return;
                }

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
                    var dic = dictionary.ToAvaloniaResourceDictionary();
                    if (dic == null)
                    {
                        continue;
                    }
                    if (dictionary.IsThemeResource())
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

                    // TODO Should dispose token's styles?
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
                
                // TODO DELETE WHEN RLS.
                Dispatcher.UIThread.Post(() => throw new Exception("Something wrong in acss. Please check the inner exception.", e));
            }
        }

        private void TokensOnFileChanged(object? sender, EventArgs e)
        {
            Dispatcher.UIThread.Invoke(() => Load(_owner, true));
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(AcssFile)} {Source}";
        }



        #region IAcssFile

        public ISource Source { get; }

        public void Reload(bool reapplyStyle)
        {
            _tokens?.OnFileChanged();
        }

        public void Unload()
        {
            this.Dispose();

            _context.TryRemoveAcssFile(this);

            // TODO What about tokens and source?
        }

        #endregion
    }
}