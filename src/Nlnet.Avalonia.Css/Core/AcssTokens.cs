using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// This keeps the raw tokens for an acss file.
/// </summary>
internal class AcssTokens : IDisposable
{
    private static AcssTokens Empty { get; } = new();

    /// <summary>
    /// Get tokens from a path. If it exists in the acss builder, just return it.
    /// </summary>
    /// <param name="acssBuilder"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static AcssTokens Get(IAcssBuilder acssBuilder, string? filePath)
    {
        if (filePath == null)
        {
            return Empty;
        }

        var standardPath = filePath.GetStandardPath();

        if (acssBuilder.TryGetAcssTokens(standardPath, out var tokens) && tokens != null)
        {
            return tokens;
        }

        if (File.Exists(standardPath) == false)
        {
            return Empty;
        }

        tokens = new AcssTokens(acssBuilder, standardPath);
        tokens.DoParsing();

        acssBuilder.TryAddAcssTokens(standardPath, tokens);

        return tokens;
    }

    private CompositeDisposable? _disposable;
    private readonly IAcssBuilder _acssBuilder;
    private List<IAcssSection>? _sections;
    private List<AcssTokens>? _imports;
    private List<AcssTokens>? _relies;
    private List<AcssTokens>? _bases;

    public string? StandardPath { get; set; }

    public event EventHandler? FileChanged;

    internal void OnFileChanged()
    {
        this.Reload();
        FileChanged?.Invoke(this, EventArgs.Empty);
    }

    

    private AcssTokens()
    {
        _acssBuilder = null!;
        StandardPath = null;
    }
    
    private AcssTokens(IAcssBuilder acssBuilder, string standardPath)
    {
        _acssBuilder = acssBuilder;
        StandardPath = standardPath;
    }

    private void DoParsing()
    {
        if (StandardPath == null || File.Exists(StandardPath) == false)
        {
            Clear();
            return;
        }

        var acssSource = File.ReadAllText(StandardPath);

        if (string.IsNullOrEmpty(acssSource))
        {
            Clear();
            return;
        }

        var parser = _acssBuilder.Parser;
        var acssSpan = parser.RemoveComments(acssSource.ToCharArray());
        
        parser.ParseImportsBasesAndRelies(acssSpan, out var imports, out var bases, out var relies, out var contentSpan);

        _imports = imports.Select(s => Get(_acssBuilder, s)).ToList();
        _relies = relies.Select(s => Get(_acssBuilder, s)).ToList();
        _bases = bases.Select(s => Get(_acssBuilder, s)).ToList();
        _sections = parser.ParseSections(null, contentSpan).ToList();

        // TODO imports and bases
        _disposable = new CompositeDisposable();
        _relies.ForEach(r =>
        {
            r.FileChanged -= OnDependencyChanged;
            r.FileChanged += OnDependencyChanged;
        });
        _disposable.Add(Disposable.Create(() =>
        {
            _relies.ForEach(r =>
            {
                r.FileChanged -= OnDependencyChanged;
            });
        }));
    }

    private void OnDependencyChanged(object? sender, EventArgs e)
    {
        FileChanged?.Invoke(sender, e);
    }

    private void Clear()
    {
        _imports = null;
        _relies = null;
        _bases = null;
        _sections = null;
        _disposable?.Dispose();
        _disposable = null;
    }



    #region Public Methods

    public void Reload()
    {
        DoParsing();
    }

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

    public void Dispose()
    {
        _disposable?.Dispose();
    }

    #endregion
}