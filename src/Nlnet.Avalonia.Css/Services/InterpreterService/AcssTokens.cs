using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class AcssTokens
{
    private static AcssTokens Empty { get; } = new();

    public static AcssTokens Get(IAcssBuilder acssBuilder, string? source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return Empty;
        }

        var tokens =  new AcssTokens(acssBuilder, source);
        tokens.DoParsing();
        return tokens;
    }

    private readonly IAcssBuilder _acssBuilder;
    private readonly string _source;
    private List<IAcssSection>? _sections;
    private List<string>? _imports;
    private List<string>? _relies;

    private AcssTokens()
    {
        _acssBuilder = null!;
        _source = string.Empty;
    }
    
    private AcssTokens(IAcssBuilder acssBuilder, string source)
    {
        _acssBuilder = acssBuilder;
        _source = source;
    }

    private void DoParsing()
    {
        var parser = _acssBuilder.Parser;
        var acssSpan = parser.RemoveComments(_source.ToCharArray());
        
        parser.ParseImportsBasesAndRelies(acssSpan, out var imports, out var bases, out var relies, out var contentSpan);
        
        _imports = imports.ToList();
        _relies = relies.ToList();
        _sections = parser.ParseSections(null, contentSpan).ToList();
    }

    private void HandleImports()
    {
        var parser = _acssBuilder.Parser;
        var acssSpan = parser.RemoveComments(_source.ToCharArray());
        
        parser.ParseImportsBasesAndRelies(acssSpan, out var imports, out var bases, out var relies, out var contentSpan);
        
        _imports = imports.ToList();
        _relies = relies.ToList();
        _sections = parser.ParseSections(null, contentSpan).ToList();

        if (_imports.Count > 0)
        {
            var builder = new StringBuilder();
            foreach (var import in _imports)
            {
                var standardPath = import.GetStandardPath();
                if (File.Exists(standardPath) == false)
                {
                    continue;
                }
                
                var source = File.ReadAllText(standardPath);
                
            }
        }
    }

    private void AppendImport(string import)
    {
        var standardPath = import.GetStandardPath();
        if (File.Exists(standardPath) == false)
        {
            return;
        }
        
        var source = File.ReadAllText(standardPath);
        var token = AcssTokens.Get(_acssBuilder, source);
        
    }

    public List<string>? GetImports()
    {
        return _imports;
    }

    public IEnumerable<IAcssFile?>? GetRelies(Styles ownerStyles)
    {
        return _relies?.Select(r =>
        {
            var standardFile = r.GetStandardPath();
            if (_acssBuilder.TryGetAcssFile(standardFile, out var file))
            {
                return file;
            }

            file = _acssBuilder.BuildLoader().Load(ownerStyles, standardFile);
            return file;
        });
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
}