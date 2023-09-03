using System.Collections.Generic;
using System.Linq;

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
        else
        {
            var tokens =  new AcssTokens(acssBuilder, source);
            tokens.DoParsing();
            return tokens;
        }
    }

    private readonly IAcssBuilder _acssBuilder;
    private readonly string _source;
    private List<IAcssSection>? _sections;

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
        
        _sections = parser.ParseSections(null, acssSpan).ToList();
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