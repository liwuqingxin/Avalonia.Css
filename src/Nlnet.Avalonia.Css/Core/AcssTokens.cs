using System;
using System.Collections.Generic;
using System.Linq;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// This keeps the raw tokens for an acss source.
/// </summary>
internal class AcssTokens : IDisposable
{
    /// <summary>
    /// Get tokens from a source. If it exists in the acss context, just return it.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static AcssTokens? Get(IAcssContext context, ISource source)
    {
        if (source.IsValid() == false)
        {
            context.OnError(AcssErrors.Source_Invalid, $"The source '{source}' is invalid. Skip it.");
            return null;
        }

        if (context.TryGetAcssTokens(source, out var tokens) && tokens != null)
        {
            return tokens;
        }

        tokens = new AcssTokens(context, source);
        tokens.DoParsing();

        context.TryAddAcssTokens(source, tokens);

        // TODO 取消事件绑定
        source.SourceChanged -= tokens.SourceOnSourceChanged;
        source.SourceChanged += tokens.SourceOnSourceChanged;
        source.Attach(context);

        return tokens;
    }

    private void SourceOnSourceChanged(object? sender, EventArgs e)
    {
        this.OnFileChanged();
    }

    private CompositeDisposable? _disposable;
    private readonly IAcssContext _context;
    private List<IAcssSection>? _sections;
    private List<AcssTokens>? _imports;
    private List<AcssTokens>? _relies;
    private List<AcssTokens>? _bases;

    public ISource Source { get; set; }

    /// <summary>
    /// Fires if the file changed.
    /// </summary>
    public event EventHandler? FileChanged;

    /// <summary>
    /// Fires if the rely file changed. This is invoked after the <see cref="FileChanged"/>.
    /// </summary>
    public event EventHandler? RelyChanged;

    internal void OnFileChanged()
    {
        this.ReloadFromFile();
        FileChanged?.Invoke(this, EventArgs.Empty);
        RelyChanged?.Invoke(this, EventArgs.Empty);
    }

    
    
    private AcssTokens(IAcssContext context, ISource source)
    {
        _context = context;
        Source   = source;
    }

    private void DoParsing()
    {
        Clear();

        var content = Source.GetSource();
        if (string.IsNullOrEmpty(content))
        {
            return;
        }

        var parser = _context.GetService<IAcssParser>();
        var acssSpan = parser.RemoveComments(content.ToCharArray());
        
        parser.ParseImportsBasesAndRelies(acssSpan, out var imports, out var bases, out var relies, out var contentSpan);

        var sourceFactory = _context.GetService<ISourceFactory>();
        _imports  = imports.Select(s => Get(_context, sourceFactory.CreateDependentSource(Source, s, true))).Where(t => t != null).ToList()!;
        _relies   = relies.Select(s => Get(_context, sourceFactory.CreateDependentSource(Source, s, true))).Where(t => t != null).ToList()!;
        _bases    = bases.Select(s => Get(_context, sourceFactory.CreateDependentSource(Source, s, true))).Where(t => t != null).ToList()!;
        _sections = parser.ParseSections(this, null, contentSpan).ToList();
        
        foreach (var acssStyle in GetStyles())
        {
            acssStyle.ForceMergeBase();
        }

        PrepareRelies();
    }

    private void PrepareRelies()
    {
        _disposable = new CompositeDisposable();
        _imports?.ForEach(r =>
        {
            r.RelyChanged -= OnImportChanged;
            r.RelyChanged += OnImportChanged;
        });
        _relies?.ForEach(r =>
        {
            r.RelyChanged -= OnRelyChanged;
            r.RelyChanged += OnRelyChanged;
        });
        _bases?.ForEach(r =>
        {
            r.RelyChanged -= OnBaseChanged;
            r.RelyChanged += OnBaseChanged;
        });
        _disposable.Add(Disposable.Create(() =>
        {
            _imports?.ForEach(r =>
            {
                r.RelyChanged -= OnImportChanged;
            });
            _relies?.ForEach(r =>
            {
                r.RelyChanged -= OnRelyChanged;
            });
            _bases?.ForEach(r =>
            {
                r.RelyChanged -= OnBaseChanged;
            });
        }));
    }

    private void OnImportChanged(object? sender, EventArgs e)
    {
        FileChanged?.Invoke(sender, e);
        RelyChanged?.Invoke(sender, e);
    }
    
    private void OnRelyChanged(object? sender, EventArgs e)
    {
        FileChanged?.Invoke(sender, e);
        RelyChanged?.Invoke(sender, e);
    }
    
    private void OnBaseChanged(object? sender, EventArgs e)
    {
        foreach (var acssStyle in GetStyles())
        {
            acssStyle.ForceMergeBase();
        }
        
        FileChanged?.Invoke(sender, e);
        RelyChanged?.Invoke(sender, e);
    }

    private void Clear()
    {
        _disposable?.Dispose();
        _disposable = null;
        _imports = null;
        _relies = null;
        _bases = null;
        _sections = null;
    }
    
    private void ReloadFromFile()
    {
        DoParsing();
    }



    #region Public Methods

    public IEnumerable<IAcssStyle> GetStyles()
    {
        return _sections?.OfType<IAcssStyle>()
               ?? Enumerable.Empty<IAcssStyle>();
    }

    public IEnumerable<IAcssStyle> GetThemeStyles()
    {
        return _sections?.OfType<IAcssStyle>().Where(s => s.IsThemeChild)
               ?? Enumerable.Empty<IAcssStyle>();
    }

    public IEnumerable<IAcssStyle> GetNormalStyles()
    {
        return _sections?.OfType<IAcssStyle>().Where(s => !s.IsThemeChild)
               ?? Enumerable.Empty<IAcssStyle>();
    }

    public IEnumerable<IAcssResourceDictionary> GetResourceDictionaries()
    {
        return _sections?.OfType<IAcssResourceDictionary>()
               ?? Enumerable.Empty<IAcssResourceDictionary>();
    }

    public IAcssStyle? TryGetBaseStyle(string key)
    {
        var s0 = GetStyles().FirstOrDefault(s => s.MatchKey(key));
        if (s0 != null)
        {
            return s0;
        }

        if (_bases == null)
        {
            return null;
        }
        
        foreach (var tokens in _bases)
        {
            var s = tokens.GetStyles().FirstOrDefault(s => s.MatchKey(key));
            if (s != null)
            {
                return s;
            }
        }

        return null;
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }

    #endregion
}